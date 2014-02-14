//
//  QuestionViewController.m
//  Conquistador
//
//  Created by Jérôme Boursier on 18/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import "QuestionViewController.h"
#import "SocketIOPacket.h"
#import "NSArray+Shuffle.h"

@interface QuestionViewController ()

// QCM
- (IBAction)answerSelected:(id)sender;
- (void)makeOtherButtonsDisappear:(UIButton *)exceptThisOne;

// Open
- (IBAction)clearPressed:(id)sender;
- (IBAction)submitPressed:(id)sender;
- (IBAction)digitPressed:(id)sender;
- (void)disableAllButtons;

//General
- (BOOL)theAnswerIsCorrect:(NSString *)theAnswer;
- (void)sendAnswer;
- (void)dissMissViewController;

// Chrono
- (void)startChrono;
- (void)updateTimer;
- (NSString *)secondsValueOfTimeString:(NSString *)chronometer;

@end

@implementation QuestionViewController
@synthesize chrono;

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

- (void)viewDidLoad
{
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
    
    self.userSocket.delegate = self;
    
    // Texts & Fonts
    titleView.font   = [UIFont titleFontWithSize:30];
    titleView2.font  = [UIFont titleFontWithSize:20];
    login.font       = [UIFont textFontWithSize:20];
    self.chrono.font = [UIFont textFontWithSize:16];
    
    // Fill
    login.text  = self.userLogin;
    
    // Background
    UIGraphicsBeginImageContext(self.view.frame.size);
    [self.userImage drawInRect:self.view.bounds];
    UIImage *bckgrnd = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    self.view.backgroundColor = [UIColor colorWithPatternImage:bckgrnd];
    
    question = [self.userQuestion valueForKey:@"title"];
    
    if(self.isQCM)
    {
        wrongAnswer1  = [self.userQuestion valueForKey:@"false1"];
        wrongAnswer2  = [self.userQuestion valueForKey:@"false2"];
        wrongAnswer3  = [self.userQuestion valueForKey:@"false3"];
        correctAnswer = [self.userQuestion valueForKey:@"answer"];
        
        [answer1Button setAlpha:1.0];
        [answer2Button setAlpha:1.0];
        [answer3Button setAlpha:1.0];
        [answer4Button setAlpha:1.0];
        
    }
    else
    {
        correctAnswer = [[self.userQuestion valueForKey:@"answer"] stringValue];
        [button0 setAlpha:1.0];
        [button1 setAlpha:1.0];
        [button2 setAlpha:1.0];
        [button3 setAlpha:1.0];
        [button4 setAlpha:1.0];
        [button5 setAlpha:1.0];
        [button6 setAlpha:1.0];
        [button7 setAlpha:1.0];
        [button8 setAlpha:1.0];
        [button9 setAlpha:1.0];
        [clearButton setAlpha:1.0];
        [submitButton setAlpha:1.0];
    }

    // Colors management
    UIColor *bgColor = nil;
    UIColor *fontColor = nil;
    switch ([self.userId intValue])
    {
        case 0:
            bgColor = [UIColor redBg];
            fontColor = [UIColor whiteColor];
            break;
        case 1:
            bgColor = [UIColor yellowBg];
            fontColor = [UIColor blackColor];
            break;
        case 2:
            bgColor = [UIColor greenBg];
            fontColor = [UIColor blackColor];
            break;
        case 3:
            bgColor = [UIColor blueBg];
            fontColor = [UIColor whiteColor];
            break;
    }
    
    [containerHeader setBackgroundColor:[bgColor colorWithAlphaComponent:0.7]];
    [containerQuestion setBackgroundColor:[bgColor colorWithAlphaComponent:0.7]];
    [containerAnswers setBackgroundColor:[bgColor colorWithAlphaComponent:0.7]];
    
    login.textColor               = fontColor;
    chrono.textColor              = fontColor;
    questionLabel.textColor       = fontColor;
    questionLabel1.textColor      = fontColor;
    openAnswer.textColor          = fontColor;
    theCorrectAnswer.textColor    = fontColor;
    theCorrectAnswerWas.textColor = fontColor;
    yourAnswer.textColor          = fontColor;
    
    containerHeader.layer.cornerRadius   = 3.0f;
    containerQuestion.layer.cornerRadius = 3.0f;
    containerAnswers.layer.cornerRadius  = 3.0f;
}

- (void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    
    if(! self.isQCM)
    {
        theCorrectAnswer.hidden = YES;
        theCorrectAnswerWas.hidden = YES;
    }
}

- (void)viewWillDisappear:(BOOL)animated
{
    [super viewWillDisappear:animated];
    
    self.userQuestion = nil;
    self.isQCM = NO;
}

- (void) viewDidAppear:(BOOL)animated
{
    questionLabel.text = question;
    if(self.isQCM)
    {
        NSArray *allButtons = @[answer1Button, answer2Button, answer3Button, answer4Button];
        NSArray *allAnswers = @[correctAnswer, wrongAnswer1, wrongAnswer2, wrongAnswer3];
        NSArray *shuffledAnswers = [allAnswers shuffledArray];
        
        for(int i = 0; i < [shuffledAnswers count]; i++)
        {
            [[allButtons objectAtIndex:i] setTitle:[shuffledAnswers objectAtIndex:i] forState:UIControlStateNormal];
        }
    }
    else
    {
        [theCorrectAnswer setText:correctAnswer];
    }
    
    chrono.text = @"00.00.00";
    running = FALSE;
    startDate = [NSDate date];
    
    [self startChrono];
}

#pragma mark - Memory Management
- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Chrono
- (void)startChrono
{
    if(!running)
    {
        running = TRUE;
        if (stopTimer == nil)
        {
            stopTimer = [NSTimer scheduledTimerWithTimeInterval:1.0/10.0 target:self selector:@selector(updateTimer) userInfo:nil repeats:YES];
        }
    }
    else
    {
        running = FALSE;
        [stopTimer invalidate];
        stopTimer = nil;
    }
}

- (void)updateTimer
{
    NSDate *currentDate = [NSDate date];
    NSTimeInterval timeInterval = [currentDate timeIntervalSinceDate:startDate];
    NSDate *timerDate = [NSDate dateWithTimeIntervalSince1970:timeInterval];
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:@"mm:ss.SS"];
    [dateFormatter setTimeZone:[NSTimeZone timeZoneForSecondsFromGMT:0.0]];
    NSString *timeString=[dateFormatter stringFromDate:timerDate];
    chrono.text = timeString;
}

- (NSString *)secondsValueOfTimeString:(NSString *)chronometer
{
    NSArray *time = [chronometer componentsSeparatedByString:@":"];
    int minutes = [[time objectAtIndex:0] intValue];
    NSLog(@"minutes : %d", minutes);
    NSArray *minutesAndSeconds = [[time objectAtIndex:1] componentsSeparatedByString:@"."];
    int seconds = [[minutesAndSeconds objectAtIndex:0] intValue];
    NSLog(@"seconds : %d", seconds);
    int milliemes = [[minutesAndSeconds objectAtIndex:1] intValue];
    NSLog(@"milliemes : %d", milliemes);
    
    int answerTime = minutes*60*1000 + seconds*1000 + milliemes*10;
    NSLog(@"temps : %d", answerTime);
    
    return [NSString stringWithFormat:@"%d", answerTime];
}

#pragma mark - QCM Treatments
- (IBAction)answerSelected:(id)sender
{
    [self startChrono];
    [self makeOtherButtonsDisappear:(UIButton *)sender];
    
    NSString *theAnswer = [(UIButton *)sender titleLabel].text;
    BOOL isRight = NO;
    if([self theAnswerIsCorrect:theAnswer]) isRight = YES;
    else isRight = NO;
    
    isRight ? NSLog(@"Correct ! Time : %@", chrono.text) : NSLog(@"Wrong... Time : %@", chrono.text);

    [self sendAnswer];
}

- (void)makeOtherButtonsDisappear:(UIButton *)exceptThisOne
{
    NSArray *possibleValues = @[answer1Button, answer2Button, answer3Button, answer4Button];
    for(UIButton *b in possibleValues)
    {
        b.enabled = NO;
        [UIButton animateWithDuration:1.0 animations:^{
            if(b != exceptThisOne) [b setAlpha:0.2];
            
            if([b.titleLabel.text isEqualToString:correctAnswer])
                [b setTitleColor:[UIColor greenColor] forState:UIControlStateNormal];
            else
                [b setTitleColor:[UIColor redColor] forState:UIControlStateNormal];
        }];
    }
}

#pragma mark - Open Treatments
- (IBAction)clearPressed:(id)sender
{
    [openAnswer setText:@"0"];
}

- (IBAction)submitPressed:(id)sender
{
    [self startChrono];
    [self disableAllButtons];
    
    BOOL isRight = NO;
    if([self theAnswerIsCorrect:openAnswer.text]) isRight = YES;
    else isRight = NO;
    
    isRight ? NSLog(@"Correct ! Time : %@", chrono.text) : NSLog(@"Wrong... Time : %@", chrono.text);
    
    theCorrectAnswer.hidden = NO;
    theCorrectAnswerWas.hidden = NO;
    
    [self sendAnswer];
}

- (IBAction)digitPressed:(id)sender
{
    NSString *digit = [(UIButton *)sender titleLabel].text;
    if([openAnswer.text isEqualToString:@"0"])
        [openAnswer setText:digit];
    else
        [openAnswer setText:[openAnswer.text stringByAppendingString:digit]];
}

- (void)disableAllButtons
{
    [button0 setEnabled:NO];
    [button1 setEnabled:NO];
    [button2 setEnabled:NO];
    [button3 setEnabled:NO];
    [button4 setEnabled:NO];
    [button5 setEnabled:NO];
    [button6 setEnabled:NO];
    [button7 setEnabled:NO];
    [button8 setEnabled:NO];
    [button9 setEnabled:NO];
    [clearButton setEnabled:NO];
    [submitButton setEnabled:NO];
}

#pragma mark - General Treatments
- (BOOL)theAnswerIsCorrect:(NSString *)theAnswer
{
    finalUserAnswer = theAnswer;
    return [theAnswer isEqualToString:correctAnswer];
}

- (IBAction)cancelPressed:(id)sender
{
    [self dismissViewControllerAnimated:YES completion:nil];
}

- (void)sendAnswer
{
    NSMutableDictionary *dict = [NSMutableDictionary dictionary];
    [dict setObject:self.userLogin forKey:@"pseudo"];
    [dict setObject:finalUserAnswer forKey:@"answer"];
    [dict setObject:[self secondsValueOfTimeString:chrono.text] forKey:@"time"];
    [dict setObject:self.questionID forKey:@"id"];
    
    [self.userSocket sendEvent:@"answer" withData:dict];
    
    [self performSelector:@selector(dissMissViewController) withObject:self afterDelay:3];

}

- (void)dissMissViewController
{
    [self dismissViewControllerAnimated:YES completion:nil];
}

#pragma mark - SocketIODelegate
- (void) socketIODidDisconnect:(SocketIO *)socket disconnectedWithError:(NSError *)error
{
    [self.navigationController popToRootViewControllerAnimated:YES];
    [[NSNotificationCenter defaultCenter] postNotificationName:@"disconnect" object:nil];
}

- (void)socketIO:(SocketIO *)socket didReceiveEvent:(SocketIOPacket *)packet
{
    NSLog(@"packet : %@", packet.dataAsJSON);
}







@end
