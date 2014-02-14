//
//  WaitingViewController.m
//  Conquistador
//
//  Created by Jérôme Boursier on 17/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import "WaitingViewController.h"
#import "QuestionViewController.h"

#import "SocketIOPacket.h"

#import "SRWebSocket.h"

@interface WaitingViewController ()

- (void)updateSlices:(NSArray *)newScores;

@end

@implementation WaitingViewController

- (id)initWithNibName:(NSString *)nibNameOrNil bundle:(NSBundle *)nibBundleOrNil
{
    self = [super initWithNibName:nibNameOrNil bundle:nibBundleOrNil];
    if (self) {
        // Custom initialization
    }
    return self;
}

#pragma mark - Views Management
- (BOOL)prefersStatusBarHidden
{
    return YES;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    
    // Texts & Fonts
    titleView.font   = [UIFont titleFontWithSize:30];
    titleView2.font  = [UIFont titleFontWithSize:20];
    firstLabel1.font = [UIFont textFontWithSize:14];
    firstLabel2.font = [UIFont textFontWithSize:14];
    firstLabel3.font = [UIFont textFontWithSize:14];
    player1.font     = [UIFont textFontWithSize:16];
    login1.font      = [UIFont textFontWithSize:16];
    score1.font      = [UIFont textFontWithSize:14];
    player2.font     = [UIFont textFontWithSize:16];
    login2.font      = [UIFont textFontWithSize:16];
    score2.font      = [UIFont textFontWithSize:14];
    player3.font     = [UIFont textFontWithSize:16];
    login3.font      = [UIFont textFontWithSize:16];
    score3.font      = [UIFont textFontWithSize:14];
    player4.font     = [UIFont textFontWithSize:16];
    login4.font      = [UIFont textFontWithSize:16];
    score4.font      = [UIFont textFontWithSize:14];
    
    // Background
    UIGraphicsBeginImageContext(self.view.frame.size);
    [[UIImage imageNamed:@"waiting_background.png"] drawInRect:self.view.bounds];
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    self.view.backgroundColor = [UIColor colorWithPatternImage:image];
    
    // Socket
    self.userSocket.delegate = self;
    
    // Colors management
    firstContainer.alpha = 0.9;
    UIGraphicsBeginImageContext(firstContainer.frame.size);
    [[UIImage imageNamed:@"parchemin.png"] drawInRect:firstContainer.bounds];
    UIImage *img = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    firstContainer.backgroundColor = [UIColor colorWithPatternImage:img];
    
    container.alpha = 1;
    UIGraphicsBeginImageContext(container.frame.size);
    [[UIImage imageNamed:@"parchemin2.png"] drawInRect:container.bounds];
    UIImage *img2 = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    container.backgroundColor = [UIColor colorWithPatternImage:img2];

    // Images Layer
    if(IS_IPHONE_5)
    {
        [[p1Color layer] setCornerRadius:25.0];
        [[p2Color layer] setCornerRadius:25.0];
        [[p3Color layer] setCornerRadius:25.0];
        [[p4Color layer] setCornerRadius:25.0];
    }
    else
    {
        [[p1Color layer] setCornerRadius:20.0];
        [[p2Color layer] setCornerRadius:20.0];
        [[p3Color layer] setCornerRadius:20.0];
        [[p4Color layer] setCornerRadius:20.0];
    }
    
    [[image1 layer] setMasksToBounds:YES];
    [[image2 layer] setMasksToBounds:YES];
    [[image3 layer] setMasksToBounds:YES];
    [[image4 layer] setMasksToBounds:YES];
    
    [[p1Color layer] setMasksToBounds:YES];
    p1Color.backgroundColor = [UIColor redBg];
    [[p2Color layer] setMasksToBounds:YES];
    p2Color.backgroundColor = [UIColor yellowBg];
    [[p3Color layer] setMasksToBounds:YES];
    p3Color.backgroundColor = [UIColor greenBg];
    [[p4Color layer] setMasksToBounds:YES];
     p4Color.backgroundColor = [UIColor blueBg];

    // Pie Chart
    slices = [NSMutableArray arrayWithCapacity:4];
    
    for(int i = 0; i < 4; i ++)
    {
        NSNumber *one = @25;
        [slices addObject:one];
    }
    
    [scoresPieChart setDelegate:self];
    [scoresPieChart setDataSource:self];
    if(IS_IPHONE_5)
        [scoresPieChart setPieCenter:CGPointMake(75, 75)];
    else
        [scoresPieChart setPieCenter:CGPointMake(50, 50)];
    
    [scoresPieChart setUserInteractionEnabled:NO];
    [scoresPieChart setShowPercentage:YES];
    [scoresPieChart setLabelColor:[UIColor blackColor]];
    [scoresPieChart setPieBackgroundColor:[UIColor clearColor]];
    
    sliceColors =[NSArray arrayWithObjects:
                  [UIColor redBg],
                  [UIColor yellowBg],
                  [UIColor greenBg],
                  [UIColor blueBg],
                  nil];
    
    firstLaunch = YES;
    [firstActivity startAnimating];
}

- (void)viewDidAppear:(BOOL)animated
{
    [super viewDidAppear:animated];
    [scoresPieChart reloadData];
    
    //[self.userSocket sendEvent:@"requestQuestionTest" withData:nil];
}

- (void)viewWillAppear:(BOOL)animated
{
    [self.navigationController setNavigationBarHidden:YES animated:animated];
    [super viewWillAppear:animated];
    
    self.userSocket.delegate = self;
    
    if(!firstLaunch)
    {
        [firstActivity stopAnimating];
        firstContainer.userInteractionEnabled = NO;
        firstContainer.hidden = YES;
        
        container.userInteractionEnabled = YES;
        container.hidden = NO;
    }
    else
    {
        container.userInteractionEnabled = NO;
        container.hidden = YES;
    }
}

- (void)viewDidUnload
{
    scoresPieChart = nil;
    [super viewDidUnload];
}

#pragma mark - Memory Management
- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - SocketIODelegate
- (void)socketIO:(SocketIO *)socket didReceiveEvent:(SocketIOPacket *)packet
{
    if([[packet.dataAsJSON valueForKey:@"name"] isEqualToString:@"question"])
    {
        firstLaunch = NO;
        NSMutableArray *questionArray = [packet.dataAsJSON objectForKey:@"args"];
        NSMutableDictionary *questionDic = [questionArray objectAtIndex:0];
        QuestionViewController *questionVC = nil;
        AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
        
        if([[questionDic valueForKey:@"type"] isEqualToString:@"qcm"])
        {
            if(IS_IPHONE_5)
                questionVC = [[QuestionViewController alloc] initWithNibName:@"QCMViewController" bundle:nil];
            else
                questionVC = [[QuestionViewController alloc] initWithNibName:@"QCMViewControllerIphone4" bundle:nil];
            
            questionVC.isQCM = YES;
        }
        else
        {
            if(IS_IPHONE_5)
                questionVC = [[QuestionViewController alloc] initWithNibName:@"OpenViewController" bundle:nil];
            else
                questionVC = [[QuestionViewController alloc] initWithNibName:@"OpenViewControllerIphone4" bundle:nil];
            
            questionVC.isQCM = NO;
        }
        questionVC.userQuestion = questionDic;
        questionVC.userLogin = self.userLogin;
        questionVC.userImage = self.userImage;
        questionVC.questionID = [questionDic valueForKey:@"id"];
        questionVC.userSocket = socket;
        questionVC.userId = self.userId;
        
        [self presentViewController:questionVC animated:YES completion:nil];

    }
    
    if([[packet.dataAsJSON valueForKey:@"name"] isEqualToString:@"majChart"])
    {
        NSLog(@"maj chart : %@", packet.dataAsJSON);
        NSMutableArray *playersArray = [packet.dataAsJSON objectForKey:@"args"];
        NSArray *pseudoLabels = @[login1, login2, login3, login4];
        NSArray *scoresLabels = @[score1, score2, score3, score4];
        NSMutableArray *territories = [[NSMutableArray alloc] init];
        
        
        NSArray *playersPseudo = [[playersArray valueForKey:@"pseudo"] objectAtIndex:0];
        NSArray *playersScore = [[playersArray valueForKey:@"score"] objectAtIndex:0];
        NSArray *playersTerritories = [[playersArray valueForKey:@"territories"] objectAtIndex:0];
        
        NSLog(@"pseudos : %@, scores : %@, territories : %@", playersPseudo, playersScore, playersTerritories);
        
        int i = 0;
        for(NSString *playerPseudo in playersPseudo)
        {
            NSString *playerPseudo = [playersPseudo objectAtIndex:i];
            NSString *playerScore = [playersScore objectAtIndex:i];
            NSLog(@"%d",[[playersTerritories objectAtIndex:i] count]);
            
            [((UILabel *)[pseudoLabels objectAtIndex:i]) setText:playerPseudo];
            [((UILabel *)[scoresLabels objectAtIndex:i]) setText:[NSString stringWithFormat:@"%@", playerScore]];
            [territories addObject:[NSNumber numberWithInt:[playerScore intValue]]];
            
            i++;
        }
        [self updateSlices:territories];
    }

}

- (void)socketIO:(SocketIO *)socket onError:(NSError *)error
{
    NSLog(@"Error \"%@\"", error);
}

- (void)socketIODidDisconnect:(SocketIO *)socket disconnectedWithError:(NSError *)error
{
    [self.navigationController popToRootViewControllerAnimated:YES];
    [[NSNotificationCenter defaultCenter] postNotificationName:@"disconnect" object:nil];
}


#pragma mark - XYPieChart Custom Methods
- (void)updateSlices:(NSArray *)newScores
{
    for(int i = 0; i < slices.count; i ++)
    {
        [slices replaceObjectAtIndex:i withObject:[newScores objectAtIndex:i]];
    }
    [scoresPieChart reloadData];
}

#pragma mark - XYPieChart Data Source
- (NSUInteger)numberOfSlicesInPieChart:(XYPieChart *)pieChart
{
    return slices.count;
}

- (CGFloat)pieChart:(XYPieChart *)pieChart valueForSliceAtIndex:(NSUInteger)index
{
    return [[slices objectAtIndex:index] intValue];
}

- (UIColor *)pieChart:(XYPieChart *)pieChart colorForSliceAtIndex:(NSUInteger)index
{
    return [sliceColors objectAtIndex:(index % sliceColors.count)];
}

#pragma mark - XYPieChart Delegate
- (void)pieChart:(XYPieChart *)pieChart willSelectSliceAtIndex:(NSUInteger)index
{
    NSLog(@"will select slice at index %d",index);
}

- (void)pieChart:(XYPieChart *)pieChart willDeselectSliceAtIndex:(NSUInteger)index
{
    NSLog(@"will deselect slice at index %d",index);
}

- (void)pieChart:(XYPieChart *)pieChart didDeselectSliceAtIndex:(NSUInteger)index
{
    NSLog(@"did deselect slice at index %d",index);
}

- (void)pieChart:(XYPieChart *)pieChart didSelectSliceAtIndex:(NSUInteger)index
{
    NSLog(@"did select slice at index %d",index);
    //self.selectedSliceLabel.text = [NSString stringWithFormat:@"$%@",[self.slices objectAtIndex:index]];
}

@end
