import React, { useEffect, useRef } from 'react';
import { View, Text, TouchableOpacity, Image, Animated, ImageBackground } from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import IntroStyles from './IntroStyles';
import { useNavigation } from '@react-navigation/native';

export default function IntroScreen() {
  const anim1 = useRef(new Animated.Value(0)).current;
  const anim2 = useRef(new Animated.Value(0)).current;
  const anim3 = useRef(new Animated.Value(0)).current;
  const logoAnim = useRef(new Animated.Value(0)).current;
  const floatAnim = useRef(new Animated.Value(0)).current;
  const starsAnim = useRef(new Animated.Value(0)).current;

  useEffect(() => {

    Animated.spring(logoAnim, {
      toValue: 1,
      tension: 20,
      friction: 7,
      useNativeDriver: true,
    }).start();

    Animated.loop(
      Animated.sequence([
        Animated.timing(floatAnim, {
          toValue: 1,
          duration: 3000,
          useNativeDriver: true,
        }),
        Animated.timing(floatAnim, {
          toValue: 0,
          duration: 3000,
          useNativeDriver: true,
        }),
      ])
    ).start();

    Animated.loop(
      Animated.sequence([
        Animated.timing(starsAnim, {
          toValue: 1,
          duration: 2000,
          useNativeDriver: true,
        }),
        Animated.timing(starsAnim, {
          toValue: 0,
          duration: 2000,
          useNativeDriver: true,
        }),
      ])
    ).start();

    Animated.stagger(150, [
      Animated.spring(anim1, { 
        toValue: 1, 
        tension: 50,
        friction: 7,
        useNativeDriver: true 
      }),
      Animated.spring(anim2, { 
        toValue: 1, 
        tension: 50,
        friction: 7,
        useNativeDriver: true 
      }),
      Animated.spring(anim3, { 
        toValue: 1, 
        tension: 50,
        friction: 7,
        useNativeDriver: true 
      }),
    ]).start();
  }, []);

  const { navigate } = useNavigation();

  function goParentSignUP() {
    navigate('ParentStack', { screen: 'ParentSignUp' });
  }

  function goParentLogin() {
    navigate('ParentStack', { screen: 'ParentLogin' });
  }

  function goChildLogin() {
    navigate('ChildStack', { screen: 'ChildLogin' });
  }

  const createAnimationStyle = (anim) => ({
    opacity: anim,
    transform: [
      {
        translateY: anim.interpolate({
          inputRange: [0, 1],
          outputRange: [50, 0],
        }),
      },
      {
        scale: anim.interpolate({
          inputRange: [0, 1],
          outputRange: [0.8, 1],
        }),
      },
    ],
  });

  const logoAnimStyle = {
    opacity: logoAnim,
    transform: [
      {
        scale: logoAnim.interpolate({
          inputRange: [0, 1],
          outputRange: [0.3, 1],
        }),
      },
      {
        translateY: floatAnim.interpolate({
          inputRange: [0, 1],
          outputRange: [0, -15],
        }),
      },
    ],
  };

  const starsAnimStyle = {
    opacity: starsAnim.interpolate({
      inputRange: [0, 0.5, 1],
      outputRange: [0.3, 1, 0.3],
    }),
  };

  return (
    <ImageBackground
      source={require('../../../../assets/images/download (45).jpg')}
      style={IntroStyles.container}
      resizeMode="cover"
    >
      <Animated.View style={[IntroStyles.starsOverlay, starsAnimStyle]}>
        <View style={IntroStyles.star} />
        <View style={[IntroStyles.star, { top: '20%', left: '80%' }]} />
        <View style={[IntroStyles.star, { top: '70%', left: '15%' }]} />
        <View style={[IntroStyles.star, { top: '50%', left: '90%' }]} />
        <View style={[IntroStyles.star, { top: '80%', left: '70%' }]} />
      </Animated.View>


      <View style={IntroStyles.topSection}>
        <Animated.View style={[IntroStyles.logoContainer, logoAnimStyle]}>
          <Image
            source={require('../../../../assets/images/ChatGPT_Image_Feb_5__2026__01_43_06_PM-removebg-preview.png')}
            style={IntroStyles.astronaut}
            resizeMode="contain"
          />
          <View style={IntroStyles.glowEffect} />
        </Animated.View>
        
        <View style={IntroStyles.titleContainer}>
          <Text style={IntroStyles.title}>Money Mirror</Text>
          <LinearGradient
            colors={['#3ca4ff', '#00f2ff']}
            start={[0, 0]}
            end={[1, 0]}
            style={IntroStyles.titleUnderline}
          />
          <Text style={IntroStyles.subtitle}>
            Money Mirror Family welcome you! 
          </Text>
        </View>
      </View>

      <View style={IntroStyles.bottomSection}>
        <Text style={IntroStyles.welcomeText}>Choose Your Mission</Text>
        
        <View style={IntroStyles.buttonContainer}>
          <Animated.View style={createAnimationStyle(anim1)}>
            <TouchableOpacity 
              onPress={goParentSignUP}
              activeOpacity={0.8}
            >
              <LinearGradient
                colors={['#5e3e8a', '#4e6bd6']}
                start={[0, 0]}
                end={[1, 1]}
                style={IntroStyles.button}
              >
                <View style={IntroStyles.buttonContent}>
                  <Text style={IntroStyles.buttonText}>Parent Sign Up</Text>
                </View>
                <View style={IntroStyles.buttonShine} />
              </LinearGradient>
            </TouchableOpacity>
          </Animated.View>

          <Animated.View style={createAnimationStyle(anim2)}>
            <TouchableOpacity 
              onPress={goParentLogin}
              activeOpacity={0.8}
            >
              <LinearGradient
                colors={['#5e3e8a', '#4e6bd6']}
                start={[0, 0]}
                end={[1, 1]}
                style={IntroStyles.button}
              >
                <View style={IntroStyles.buttonContent}>
                  <Text style={IntroStyles.buttonText}>Parent Log In</Text>
                </View>
                <View style={IntroStyles.buttonShine} />
              </LinearGradient>
            </TouchableOpacity>
          </Animated.View>

          <Animated.View style={createAnimationStyle(anim3)}>
            <TouchableOpacity 
              onPress={goChildLogin}
              activeOpacity={0.8}
            >
              <LinearGradient
                colors={['#5e3e8a', '#4e6bd6']}
                start={[0, 0]}
                end={[1, 1]}
                style={IntroStyles.button}
              >
                <View style={IntroStyles.buttonContent}>
                  <Text style={IntroStyles.buttonText}>Child Log In</Text>
                </View>
                <View style={IntroStyles.buttonShine} />
              </LinearGradient>
            </TouchableOpacity>
          </Animated.View>
        </View>
      </View>
    </ImageBackground>
  );
}