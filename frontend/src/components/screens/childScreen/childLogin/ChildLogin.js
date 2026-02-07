// import { useState } from 'react';
// import { View, Text, TouchableOpacity, Image, ImageBackground } from 'react-native';
// import { LinearGradient } from 'expo-linear-gradient';
// import { StackActions, useNavigation } from '@react-navigation/native';
// import AntDesign from '@expo/vector-icons/AntDesign';
// import AsyncStorage from '@react-native-async-storage/async-storage';
// import axios from 'axios';
// import Toast from 'react-native-toast-message';
// import styles from './ChildLoginStyles.js';
// import { TextInput, Provider as PaperProvider } from 'react-native-paper';
// import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
// import { UseChildStore } from '../../../../store/UseChildStore.js';
// import Loading from '../../../screens/loading/Loading';

// const API_BASE_URL = 'https://localhost:7048';

// export default function ChildLogin() {
//   const [code, setCode] = useState('');
//   const [loading, setLoading] = useState(false);
//   const { goBack } = useNavigation();
//   const navigation = useNavigation();

//   async function handleLogin() {
//     if (!code.trim()) {
//       Toast.show({
//         type: 'error',
//         text2: 'Please enter your login code',
//         text2Style: { color: "black", fontSize: 15 },
//         position: 'top',
//       });
//       return;
//     }

//     const codeRegex = /^[A-Z0-9]{6}$/;
//     if (!codeRegex.test(code.trim())) {
//       Toast.show({
//         type: 'error',
//         text2: 'Code must be 6 uppercase letters/numbers',
//         text2Style: { color: "black", fontSize: 15 },
//         position: 'top',
//       });
//       return;
//     }

//     setLoading(true);

//     try {
//       const response = await axios.post(`${API_BASE_URL}/api/children/login-with-code`, {
//         code: code.trim().toUpperCase()
//       });

//       if (response.data.success) {
//         const {
//           accessToken,
//           refreshToken,
//           accessTokenExpiration,
//           refreshTokenExpiration,
//           childId,
//           childFirstName,
//           age,
//           isPersonalityFinalized,
//           personalityProfile
//         } = response.data.data;

//         await AsyncStorage.multiSet([
//           ['childAccessToken', accessToken],
//           ['childRefreshToken', refreshToken],
//           ['childAccessTokenExpiration', accessTokenExpiration],
//           ['childRefreshTokenExpiration', refreshTokenExpiration],
//           ['childId', childId.toString()],
//         ]);

//         UseChildStore.getState().setCurrentChild({
//           childId,
//           childFirstName,
//           age,
//           isPersonalityFinalized,
//           personalityProfile,
//           accessToken,
//           refreshToken
//         });

//         navigation.dispatch(
//           StackActions.replace("ChildMainTabs", {
//             state: {
//               index: 0,
//               routes: [{
//                 name: "ChildHomeStack",
//                 state: {
//                   index: 0,
//                   routes: [{
//                     name: "ChildHomeScreen",
//                     params: {
//                       child: {
//                         childId,
//                         childFirstName,
//                         age,
//                         isPersonalityFinalized,
//                         personalityProfile
//                       }
//                     }
//                   }]
//                 }
//               }]
//             }
//           })
//         );

//         Toast.show({
//           type: 'success',
//           text2: response.data.message || `Welcome back, ${childFirstName}!`,
//           text2Style: { color: "black", fontSize: 15 },
//           position: 'top',
//         });
//       }
//     } catch (error) {
//       console.error('Child login error:', error);

//       let errorMessage = 'An error occurred. Please try again.';

//       if (error.response) {
//         if (error.response.status === 401) {
//           errorMessage = 'Invalid or expired code. Please check with your parent.';
//         } else if (error.response.data?.message) {
//           errorMessage = error.response.data.message;
//         }
//       } else if (error.request) {
//         errorMessage = 'Cannot connect to server. Please check your internet connection.';
//       }

//       Toast.show({
//         type: 'error',
//         text2: errorMessage,
//         text2Style: { color: "black", fontSize: 15,flexWrap:'wrap' },
//         position: 'top',
//       });
//     } finally {
//       setLoading(false);
//     }
//   }

//   return (
//     <View style={{ flex: 1 }}>
//       <Loading visible={loading} message="Logging in..." />

//       <KeyboardAwareScrollView
//         style={{ flex: 1 }}
//         contentContainerStyle={{ flexGrow: 1 }}
//         enableOnAndroid
//         keyboardShouldPersistTaps="handled"
//       >
//         <View style={{ flex: 1 }}>
//           <ImageBackground
//             source={require('../../../../../assets/images/download (26).jpg')}
//             style={styles.container}
//           >
//             <View style={styles.arrow}>
//               <AntDesign
//                 name="arrow-left"
//                 size={24}
//                 color="white"
//                 onPress={goBack}
//               />
//             </View>

//             <View>
//               <Image
//                 source={require("../../../../../assets/images/output-onlinegiftools (12) (1).gif")}
//                 style={styles.img}
//               />
//             </View>

//             <View style={styles.form}>
//               <View>
//                 <Text style={styles.title}>Log in</Text>
//                 <Text style={styles.subtitle}>
//                   Ready to take off? Your adventure starts now!
//                 </Text>
//               </View>

//               <View style={styles.inputContainer}>
//                 <PaperProvider>
//                   <TextInput
//                     style={styles.input}
//                     placeholder="Enter your code"
//                     value={code}
//                     onChangeText={(text) => setCode(text.toUpperCase())}
//                     label="Login Code"
//                     mode="outlined"
//                     autoCapitalize="characters"
//                     maxLength={6}
//                     editable={!loading}
//                     disabled={loading}
//                   />
//                 </PaperProvider>
//               </View>

//               <View style={styles.buttonContainer}>
//                 <TouchableOpacity
//                   onPress={handleLogin}
//                   disabled={loading}
//                   activeOpacity={0.8}
//                 >
//                   <LinearGradient
//                     colors={loading ? ['#CCCCCC', '#999999'] : ['#FFD966', '#FFC107']}
//                     start={[0, 0]}
//                     end={[1, 0]}
//                     style={styles.button}
//                   >
//                     <Text style={styles.buttonText}>Take off!</Text>
//                   </LinearGradient>
//                 </TouchableOpacity>
//               </View>
//             </View>
//           </ImageBackground>
//         </View>
//       </KeyboardAwareScrollView>
//     </View>
//   );
// }



import { useState } from 'react';
import { View, Text, TouchableOpacity, ScrollView, KeyboardAvoidingView, Platform, Image, Animated, ImageBackground} from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import { StackActions, useNavigation } from '@react-navigation/native';
import AntDesign from '@expo/vector-icons/AntDesign';
import axios from 'axios';
import Toast from 'react-native-toast-message';
import styles from './ChildLoginStyles.js';
import { TextInput, Provider as PaperProvider } from 'react-native-paper';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import { UseChildStore } from '../../../../store/UseChildStore.js';

export default function ChildLogin() {
  const [code, setCode] = useState('');
  const { goBack } = useNavigation();
  const navigation = useNavigation();

  const child={
    name:"haboba",
    age:7,
    balance:34,
    type:"smart spender",
    typeDesc:"you spending smartly and in good manner",
    avatar:'Nova',
    notification:[
      {id:1,msg:"hello habooba"},
      {id:2,msg:"welcome on our journey,we are going to discover your area"},
    ],
    insights:[
      {id:1,adv:"you are good on spending"},
      {id:2,adv:"you are good on saving money"},
      {id:3,adv:"you are emotional spending"},
    ],
    recentPurchases : [
    { id: 1, name: 'Toy Car', cost: '$15.99', category: 'toys', mood: 'excited' },
    { id: 2, name: 'Ice Cream', cost: '$4.50', category: 'food', mood: 'happy' },
    ],
    purchases : [
    { id: 1, name: 'Toy Car', cost: '$15.99', category: 'toys', mood: 'excited', date: '2025-01-10' },
    { id: 2, name: 'Book', cost: '$4.50', category: 'food', mood: 'happy', date: '2025-01-12' },
    { id: 3, name: 'Ice Cream', cost: '$6.50', category: 'food', mood: 'sad', date: '2025-01-15' },
    { id: 4, name: 'Ice Cream', cost: '$8.50', category: 'food', mood: 'okay', date: '2025-01-18' }
  ]
  }

  function handleLogin() {
    if(code=='1234'){
    UseChildStore.getState().setCurrentChild(child);
    navigation.dispatch(
      StackActions.replace("ChildMainTabs",
        {state:{
          index:0,
          routes:[{
            name:"ChildHomeStack",
            state:{
              index:0,
              routes:[{
                name:"ChildHomeScreen",
                params:{child}
              }]
            }
          }]
        }
        }
      )
    )
    }else{
      Toast.show({
        type: 'error',
        text2: 'Invalid code',
        text2Style:{color:"black",fontSize:16},
        position: 'top',
      });
    }
  }


  

  return (
    <View style={{flex:1}}>
    <KeyboardAwareScrollView style={{ flex: 1 }}
      contentContainerStyle={{ flexGrow: 1 }}
      enableOnAndroid 
      keyboardShouldPersistTaps="handled" 
       >
        <View style={{flex:1}}>
          <ImageBackground source={require('../../../../../assets/images/download (26).jpg')} style={styles.container}>

          <View style={styles.arrow}>
            <AntDesign name="arrow-left" size={24} color="white" onPress={goBack}/>
          </View>

          <View >
            <Image source={require("../../../../../assets/images/output-onlinegiftools (12) (1).gif")} style={styles.img}/>
          </View>

          <View style={styles.form}>
          <View >
              <Text style={styles.title}>Log in</Text>
              <Text style={styles.subtitle}>Ready to take off? Your adventure starts now!</Text>
          </View>
            <View style={styles.inputContainer}>
                <PaperProvider>
                <TextInput style={styles.input} placeholder="Enter your code" keyboardType="numeric" value={code} onChangeText={setCode} label="code" mode="outlined"/>
                </PaperProvider>
            </View>

            <View style={styles.buttonContainer}>
                <TouchableOpacity onPress={()=>handleLogin()}>
                  <LinearGradient colors={['#FFD966', '#FFC107']}  start={[0,0]} end={[1,0]} style={styles.button}>
                    <Text style={styles.buttonText}>Take off !</Text> 
                  </LinearGradient>
                </TouchableOpacity>
            </View>
          </View>
          </ImageBackground>
        </View>
        </KeyboardAwareScrollView>
    </View>
  );
}




