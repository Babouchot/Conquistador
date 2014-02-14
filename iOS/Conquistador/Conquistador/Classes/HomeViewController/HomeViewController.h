//
//  HomeViewController.h
//  Conquistador
//
//  Created by polytech on 10/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "SocketIO.h"
#import "ZBarSDK.h"
#import <AudioToolbox/AudioToolbox.h>

@interface HomeViewController : UIViewController <UITextFieldDelegate, UIActionSheetDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate, SocketIODelegate, ZBarReaderDelegate>
{
    IBOutlet UITextField *userLoginTextField;
    IBOutlet UILabel     *userError;
    
    // Button
    IBOutlet UIButton *fightButton;
    
    // Act
    IBOutlet UIActivityIndicatorView *activity;
    
    // Font style
    IBOutlet UILabel *titleView;
    IBOutlet UILabel *titleView2;
    IBOutlet UILabel *whatsYourName;
    
    NSString *pseudo;
    
    NSMutableArray *messages;
    
    SocketIO *userSocket;
    
    ZBarReaderViewController *reader;
}

@end
