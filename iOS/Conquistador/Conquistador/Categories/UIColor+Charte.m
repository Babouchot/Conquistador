//
//  UIColor+Charte.m
//  Conquistador
//
//  Created by polytech on 10/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import "UIColor+Charte.h"

@implementation UIColor (Charte)

+ (UIColor *)backgroundColor
{
    // rgb(41, 128, 185)
    return [UIColor colorWithRed:41/255.0f green:128/255.0f blue:185/255.0f alpha:1.0];
}

+ (UIColor *)liteBackgroundColor
{
    // rgb(52, 152, 219)
    //rgb(192, 57, 43)
    return [UIColor colorWithRed:192/255.0f green:57/255.0f blue:43/255.0f alpha:0.4];
}

+ (UIColor *)textColor
{
    // rgb(44, 62, 80)
    return [UIColor colorWithRed:44/255.0f green:62/255.0f blue:80/255.0f alpha:1.0];
}

+ (UIColor *)backgroundViewColor
{
    // rgb(236, 240, 241)
    return [UIColor colorWithRed:236/255.0f green:240/255.0f blue:241/255.0f alpha:1.0];
}

+ (UIColor *)redBg
{
    //rgb(231, 76, 60)
    return [UIColor colorWithRed:231/255.0f green:76/255.0f blue:60/255.0f alpha:1.0];
}

+ (UIColor *)yellowBg
{
    //rgb(241, 196, 15)
    return [UIColor colorWithRed:241/255.0f green:196/255.0f blue:15/255.0f alpha:1.0];
}
+ (UIColor *)greenBg
{
    //rgb(46, 204, 113)
    return [UIColor colorWithRed:46/255.0f green:204/255.0f blue:113/255.0f alpha:1.0];
}
+ (UIColor *)blueBg
{
    //rgb(52, 152, 219)
    return [UIColor colorWithRed:52/255.0f green:152/255.0f blue:219/255.0f alpha:1.0];
}

@end
