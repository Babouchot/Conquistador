//
//  HomeViewController.m
//  Conquistador
//
//  Created by polytech on 10/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import "HomeViewController.h"
#import "WaitingViewController.h"

#import "SocketIOPacket.h"

@interface HomeViewController ()

// IBActions
- (IBAction)fightClick:(id)sender;

// Text Field
- (void)animateTextField:(UITextField*)textField up:(BOOL)up;

// Connection Methods
- (void)connectToHost:(NSString *)theHost;
- (BOOL)checkHost:(NSString *)theHost;

// NSNotification
- (void)serverIsDown:(NSNotification *)notification;

@end

@implementation HomeViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

#pragma mark - View Management
- (BOOL)prefersStatusBarHidden
{
    return YES;
}

- (void)viewWillAppear:(BOOL)animated
{
    [self.navigationController setNavigationBarHidden:YES animated:animated];
    [super viewWillAppear:animated];
}

- (void)viewWillDisappear:(BOOL)animated
{
    [self.navigationController setNavigationBarHidden:NO animated:animated];
    [super viewWillDisappear:animated];
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // Texts & Fonts
    titleView.font          = [UIFont titleFontWithSize:30];
    titleView2.font         = [UIFont titleFontWithSize:20];
    whatsYourName.font      = [UIFont textFontWithSize:20];
    userLoginTextField.font = [UIFont textFontWithSize:20];
    userError.textColor     = [UIColor redColor];
    userError.font          = [UIFont textFontWithSize:16];
    userError.hidden        = YES;
    
    // Buttons
    UIImage *buttonImage = [UIImage imageNamed:@"home_button.png"];
    [fightButton setImage:buttonImage forState:UIControlStateNormal];
    
    // Background
    UIGraphicsBeginImageContext(self.view.frame.size);
    [[UIImage imageNamed:@"home_background.jpg"] drawInRect:self.view.bounds];
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    self.view.backgroundColor = [UIColor colorWithPatternImage:image];
    
    // Socket
    userSocket = [[SocketIO alloc] initWithDelegate:self];
    
    // Activity
    activity.hidesWhenStopped = YES;
 
    // Notification
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(serverIsDown:)
                                                 name:@"disconnect"
                                               object:nil];
}

#pragma mark - UITextFieldDelegate
- (BOOL)textField:(UITextField *)textField shouldChangeCharactersInRange:(NSRange)range replacementString:(NSString *)string
{
    NSUInteger newLength = [textField.text length] + [string length] - range.length;
    return (newLength > 10) ? NO : YES;
}

- (void)textFieldDidBeginEditing:(UITextField *)textField
{
    [self animateTextField:textField up:YES];
    userError.hidden = YES;
}

- (void)textFieldDidEndEditing:(UITextField *)textField
{
    [self animateTextField:textField up:NO];
    userError.hidden = YES;
}

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [textField resignFirstResponder];
    userError.hidden = YES;
    return YES;
}

- (void)touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event
{    
    [userLoginTextField resignFirstResponder];
}

- (void)animateTextField:(UITextField*)textField up:(BOOL)up
{
    int movementDistance = -100;
    
    const float movementDuration = 0.3f;
    
    int movement = (up ? movementDistance : -movementDistance);
    
    [UIView beginAnimations: @"animateTextField" context: nil];
    [UIView setAnimationBeginsFromCurrentState: YES];
    [UIView setAnimationDuration: movementDuration];
    self.view.frame = CGRectOffset(self.view.frame, 0, movement);
    [UIView commitAnimations];
}

#pragma mark - UIImagePickerControllerDelegate
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    if([(NSDictionary *)[info objectForKey:ZBarReaderControllerResults] count] > 0)
    {
        [activity startAnimating];
        AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
        ZBarSymbolSet *symbolSet = [info objectForKey:ZBarReaderControllerResults];
        ZBarSymbol    *symbol    = nil;
        NSString      *qrcode    = nil;
        
        for(symbol in symbolSet)
            qrcode = [NSString stringWithString:symbol.data];
        
        [reader dismissViewControllerAnimated:YES completion:nil];
        [self connectToHost:qrcode];
        fightButton.enabled = NO;
    }
}

#pragma mark - Connection Methods
- (void)connectToHost:(NSString *)theHost
{
    if([self checkHost:theHost])
    {
        NSString *host = nil;
        NSString *port = nil;
        
        NSArray *IP = [[theHost substringFromIndex:7] componentsSeparatedByString:@":"];
        host = [IP objectAtIndex:0];
        port = [IP objectAtIndex:1];
        
        [userSocket connectToHost:host onPort:[port intValue]];
    }
    else
    {
        userError.text = @"This ain't a valid QRCode !";
        userError.hidden = NO;
        [activity stopAnimating];
        fightButton.enabled = YES;
    }
    
}

- (BOOL)checkHost:(NSString *)theHost
{
    if(! [[theHost substringToIndex:7] isEqualToString:@"http://"])
        return NO;
    if(! ([[theHost substringFromIndex:MAX((int)[theHost length]-5, 0)] isEqualToString:@":8080"] ||
          [[theHost substringFromIndex:MAX((int)[theHost length]-6, 0)] isEqualToString:@":8080/"]) )
        return NO;
    if([[theHost componentsSeparatedByString: @":"] count] != 3)
        return NO;
    if([[theHost componentsSeparatedByString: @"."] count] != 4)
        return NO;
    
    return YES;
}

#pragma mark - SocketIODelegate
- (void)socketIO:(SocketIO *)socket didReceiveEvent:(SocketIOPacket *)packet
{
    if([[packet.dataAsJSON valueForKey:@"name"] isEqualToString:@"requestIdentity"])
    {
        NSMutableDictionary *dict = [NSMutableDictionary dictionary];
        [dict setObject:pseudo forKey:@"pseudo"];
        [socket sendEvent:@"playerIdentity" withData:dict];
    }
    
    if([[packet.dataAsJSON valueForKey:@"name"] isEqualToString:@"successfullyConnected"])
    {
        [activity stopAnimating];
        fightButton.enabled = YES;
        
        WaitingViewController *waitingVC = nil;
        if(IS_IPHONE_5)
            waitingVC = [[WaitingViewController alloc] initWithNibName:@"WaitingViewController" bundle:nil];
        else
            waitingVC = [[WaitingViewController alloc] initWithNibName:@"WaitingViewControllerIphone4" bundle:nil];
        
        NSMutableArray *userArray = [packet.dataAsJSON objectForKey:@"args"];
        NSMutableDictionary *userDic = [userArray objectAtIndex:0];
        NSString *userId = [userDic valueForKey:@"id"];
        switch([userId intValue])
        {
            case 0:
                waitingVC.userImage = [UIImage imageNamed:@"Conqui1.png"];
                break;
            case 1:
                waitingVC.userImage = [UIImage imageNamed:@"Conqui2.png"];
                break;
            case 2:
                waitingVC.userImage = [UIImage imageNamed:@"Conqui3.png"];
                break;
            case 3:
                waitingVC.userImage = [UIImage imageNamed:@"Conqui4.png"];
                break;
        }
        
        waitingVC.userLogin  = pseudo;
        waitingVC.userSocket = socket;
        waitingVC.userId = userId;
        
        [self.navigationController pushViewController:waitingVC animated:YES];
    }

}

- (void)socketIO:(SocketIO *)socket onError:(NSError *)error
{
    userError.text = @"Server unreachable...";
    userError.hidden = NO;
    [activity stopAnimating];
    fightButton.enabled = YES;
}

- (void)socketIODidDisconnect:(SocketIO *)socket disconnectedWithError:(NSError *)error
{
    [self.navigationController popToRootViewControllerAnimated:YES];
}

#pragma mark - Memory management
- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - IBActions
- (IBAction)fightClick:(id)sender
{
    [userLoginTextField resignFirstResponder];
    userError.hidden = YES;
    
    NSCharacterSet *whitespace = [NSCharacterSet whitespaceAndNewlineCharacterSet];
    NSString *login = [userLoginTextField.text stringByTrimmingCharactersInSet:whitespace];
    
    if(login.length == 0)
    {
        userError.text = @"Please tell me your name !";
        userError.hidden = NO;
        
    }
    else
    {
        userError.hidden = YES;
        pseudo = login;
        
        reader = [ZBarReaderViewController new];
        reader.readerDelegate = self;
        
        [reader.scanner setSymbology:0
                              config:ZBAR_CFG_ENABLE
                                  to:0];
        [reader.scanner setSymbology:ZBAR_QRCODE
                              config:ZBAR_CFG_ENABLE
                                  to:1];
        reader.readerView.zoom = 1.0;
        reader.showsCameraControls = NO;
        UIView * infoButton = [[[[[reader.view.subviews objectAtIndex:1] subviews] objectAtIndex:0] subviews] objectAtIndex:3];
        [infoButton setHidden:YES];
        
        [self presentViewController:reader
                           animated:YES
                         completion:nil];
        
        
    }
}

#pragma mark - NSNotification
- (void) serverIsDown:(NSNotification *)notification
{
    if ([[notification name] isEqualToString:@"disconnect"])
    {
        userSocket = [[SocketIO alloc] initWithDelegate:self];
        [userError setText:@"Server disconnected..."];
        userError.hidden = NO;
    }
}

@end