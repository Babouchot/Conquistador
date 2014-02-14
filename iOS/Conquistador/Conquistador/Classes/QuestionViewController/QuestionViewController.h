//
//  QuestionViewController.h
//  Conquistador
//
//  Created by Jérôme Boursier on 18/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "SocketIO.h"
#import <AudioToolbox/AudioToolbox.h>

@interface QuestionViewController : UIViewController <SocketIODelegate> {
    // QCM
    NSString *correctAnswer;
    NSString *wrongAnswer1;
    NSString *wrongAnswer2;
    NSString *wrongAnswer3;
    
    // Open
    NSString *digitAnswer;
    
    // General
    NSString *question;
    UILabel  *chrono;
    NSTimer  *stopTimer;
    NSDate   *startDate;
    BOOL      running;

    NSString *finalUserAnswer;
    
    // General
    IBOutlet UILabel     *login;
    IBOutlet UIView      *containerHeader;
    IBOutlet UIView      *containerQuestion;
    IBOutlet UIView      *containerAnswers;
    IBOutlet UILabel     *questionLabel1;
    
    // QCM
    IBOutlet UILabel     *questionLabel;
    IBOutlet UIButton    *answer1Button;
    IBOutlet UIButton    *answer2Button;
    IBOutlet UIButton    *answer3Button;
    IBOutlet UIButton    *answer4Button;
    
    // Open
    IBOutlet UIButton    *button1;
    IBOutlet UIButton    *button2;
    IBOutlet UIButton    *button3;
    IBOutlet UIButton    *button4;
    IBOutlet UIButton    *button5;
    IBOutlet UIButton    *button6;
    IBOutlet UIButton    *button7;
    IBOutlet UIButton    *button8;
    IBOutlet UIButton    *button9;
    IBOutlet UIButton    *button0;
    IBOutlet UIButton    *clearButton;
    IBOutlet UIButton    *submitButton;
    IBOutlet UILabel     *yourAnswer;
    IBOutlet UILabel     *openAnswer;
    IBOutlet UILabel     *theCorrectAnswerWas;
    IBOutlet UILabel     *theCorrectAnswer;
    
    // Font style
    IBOutlet UILabel *titleView;
    IBOutlet UILabel *titleView2;
}

@property (nonatomic, strong) IBOutlet UILabel    *chrono;
@property (nonatomic)         BOOL                 isQCM;
@property (nonatomic, strong) NSMutableDictionary *userQuestion;
@property (nonatomic, strong) NSString            *userLogin;
@property (nonatomic, strong) NSString            *userId;
@property (nonatomic, strong) UIImage             *userImage;
@property (nonatomic, strong) SocketIO            *userSocket;
@property (nonatomic, strong) NSString            *questionID;

@end