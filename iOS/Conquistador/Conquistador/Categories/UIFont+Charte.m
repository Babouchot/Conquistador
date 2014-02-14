//
//  UIFont+Charte.m
//  Conquistador
//
//  Created by Jérôme Boursier on 24/01/2014.
//  Copyright (c) 2014 IHM. All rights reserved.
//

#import "UIFont+Charte.h"

@implementation UIFont (Charte)



+ (UIFont *)titleFontWithSize:(CGFloat)size
{
    return [UIFont fontWithName:@"Ruritania" size:size];
}

+ (UIFont *)textFontWithSize:(CGFloat)size
{
    return [UIFont fontWithName:@"HarabaraHand" size:size];
}

@end
