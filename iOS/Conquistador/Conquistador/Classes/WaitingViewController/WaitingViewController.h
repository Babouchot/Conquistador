//
//  WaitingViewController.h
//  Conquistador
//
//  Created by Jérôme Boursier on 17/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <AudioToolbox/AudioToolbox.h>
#import "SocketIO.h"
#import "XYPieChart.h"

@interface WaitingViewController : UIViewController <SocketIODelegate, XYPieChartDataSource, XYPieChartDelegate>{
    IBOutlet UIImageView             *image1;
    IBOutlet UILabel                 *login1;
    IBOutlet UILabel                 *score1;
    IBOutlet UIImageView             *image2;
    IBOutlet UILabel                 *login2;
    IBOutlet UILabel                 *score2;
    IBOutlet UIImageView             *image3;
    IBOutlet UILabel                 *login3;
    IBOutlet UILabel                 *score3;
    IBOutlet UIImageView             *image4;
    IBOutlet UILabel                 *login4;
    IBOutlet UILabel                 *score4;
    
    IBOutlet UILabel                 *player1;
    IBOutlet UILabel                 *player2;
    IBOutlet UILabel                 *player3;
    IBOutlet UILabel                 *player4;
    
    IBOutlet UILabel                 *titleView;
    IBOutlet UILabel                 *titleView2;
    IBOutlet UIView                  *container;
    IBOutlet UIView                  *firstContainer;
    IBOutlet UILabel                 *firstLabel1;
    IBOutlet UILabel                 *firstLabel2;
    IBOutlet UILabel                 *firstLabel3;
    IBOutlet UIActivityIndicatorView *firstActivity;
    
    IBOutlet UIView                  *p1Color;
    IBOutlet UIView                  *p2Color;
    IBOutlet UIView                  *p3Color;
    IBOutlet UIView                  *p4Color;
    
    IBOutlet XYPieChart              *scoresPieChart;
    
    NSMutableArray *slices;
    NSArray        *sliceColors;
    BOOL            firstLaunch;
}

@property (nonatomic, strong) NSString *userLogin;
@property (nonatomic, strong) UIImage  *userImage;
@property (nonatomic, strong) NSString *userId;
@property (nonatomic, strong) SocketIO *userSocket;


@end
