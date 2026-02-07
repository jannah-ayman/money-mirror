import {Text,StyleSheet,Animated,Image,ImageBackground,View} from 'react-native'
import { useEffect, useRef } from 'react'
import { useNavigation } from '@react-navigation/native'
import { LinearGradient } from 'expo-linear-gradient'

export default function SplashScreen() {
  const navigation = useNavigation()

  const scaleAnim = useRef(new Animated.Value(0.8)).current
  const fadeAnim = useRef(new Animated.Value(0)).current
  const slideAnim = useRef(new Animated.Value(30)).current

  useEffect(() => {
    Animated.sequence([
      Animated.parallel([
        Animated.timing(scaleAnim, {
          toValue: 1,
          duration: 1200,
          useNativeDriver: true,
        }),
        Animated.timing(fadeAnim, {
          toValue: 1,
          duration: 1200,
          useNativeDriver: true,
        }),
      ]),
      Animated.timing(slideAnim, {
        toValue: 0,
        duration: 1500,
        useNativeDriver: true,
      }),
    ]).start()

    const timer = setTimeout(() => {
      navigation.replace('Intro')
    }, 2300)

    return () => clearTimeout(timer)
  }, [])

  return (
    <ImageBackground source={require('../../../../assets/images/download (45).jpg')} style={styles.container} resizeMode="cover">
    {/* // <LinearGradient colors={['#0B0F2A', '#1B1F4A', '#3A3F7A']} style={styles.container}> */}
      <Animated.View
        style={{ alignItems: 'center', transform: [{ scale: scaleAnim }], opacity: fadeAnim, marginTop:50}}>
        <View >
        <Text style={styles.title}>
          Money <Text style={styles.mirror}>Mirror</Text>
        </Text>
        <Text style={styles.reflection}>
          Money Mirror
        </Text>
        </View>
      </Animated.View>
      {/* <Image source={require("../../../../assets/images/fly5.gif")} style={styles.planet}/> */}
    {/* // </LinearGradient> */}
    </ImageBackground>
  )
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent:"center",
    alignItems:"center",
  },
  title: {
    fontSize: 60,
    letterSpacing: 1,
    color: '#ffffff',
    fontFamily: 'MouseMemoirs-Regular'
  },
  mirror: {
    opacity: 0.9,
  },
  reflection: {
    fontSize: 60,
    letterSpacing: 1,
    fontFamily: 'MouseMemoirs-Regular',
    color: '#ffffff',
    opacity: 0.4,
    transform: [{ scaleY: -1 }],
    marginTop: 6,
  },
  // subtitle: {
  //   marginTop: 18,
  //   fontSize: 25,
  //   color: '#ffffff',
  //   letterSpacing: 0.8,
  //   fontFamily: 'MouseMemoirs-Regular',
  // },

  planet: {
    width: 300,
    height: 300,
  },
})
