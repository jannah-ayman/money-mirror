// import React, { useState } from 'react';
// import {View,Text,TouchableOpacity, ImageBackground } from 'react-native';
// import { LinearGradient } from 'expo-linear-gradient';
// import { useNavigation } from '@react-navigation/native';
// import AntDesign from '@expo/vector-icons/AntDesign';
// import axios from 'axios';
// import Toast from 'react-native-toast-message';
// import styles from './ParentLoginStyles.js';
// import { TextInput, Provider as PaperProvider } from 'react-native-paper';
// import { StackActions } from "@react-navigation/native";
// import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';

// export default function ParentLogin() {

//   const [email, setEmail] = useState('');
//   const [password, setPassword] = useState('');

//   const { goBack } = useNavigation();
//   const navigate = useNavigation();

//   // async function handleLogin() {
//   //   try {
//   //     if (!email || !password) {
//   //       Toast.show({
//   //         type: 'error',
//   //         text2: 'Please fill all the fields',
//   //         text2Style: { color: 'black', fontSize: 16 },
//   //         position: 'top',
//   //       });
//   //       return;
//   //     }
//   //     const res = await axios.post('url', { email: email,password: password});
//   //     const user = res.data;

//   //     if (user) {
//   //       Toast.show({
//   //         type: 'success',
//   //         text2: `Welcome back ${user.fName}`,
//   //         text2Style: { fontSize: 15, color: 'black' },
//   //         position: 'top',
//   //       });
//   //       navigate('ParentStack', {screen: 'ParentHomeScreen', params: { user }});
//   //     }
//   //   } catch (error) {
//   //     console.log(error.response?.data || error.message);
//   //     Toast.show({
//   //       type: 'error',
//   //       text2: 'Login failed, try again',
//   //       text2Style: { fontSize: 15, color: 'black' },
//   //       position: 'top',
//   //     });
//   //   }
//   // }

//   function handleLogin() {
//     if(email=='arwa@gmail.com' || email=='doha@gmail.com'){
//       if(password=='1234'){
//         navigate.dispatch(
//           StackActions.replace("ParentMainTabs",{
//             screen:"ParentStack",
//             params:{
//               screen:"ParentHomeScreen",
//             }
//           }
//           )
//         )
//       }
//     }else{
//       Toast.show({
//         type: 'error',
//         text2: 'Invalid email or password',
//         text2Style:{color:"black",fontSize:16},
//         position: 'top',
//       });
//     }
//   }

//   function goSignUp() {
//     navigate('ParentStack', { screen: 'ParentSignUp' });
//   }

//   return (
//     <KeyboardAwareScrollView style={{ flex: 1 }}
//         contentContainerStyle={{ flexGrow: 1, justifyContent: 'center' }}
//         enableOnAndroid={true} 
//         extraScrollHeight={5}
//         keyboardShouldPersistTaps="handled">
//         <View style={{flexGrow:1}}>
//         <ImageBackground source={require('../../../../../assets/images/download (11).jpg')} style={styles.container}>
//         <View style={styles.arrow}>
//           <AntDesign name="arrow-left" size={24} color="white" onPress={goBack}/>
//         </View>
//           <View style={styles.form}>
//             <View>
//               <Text style={styles.title}>Parent Log in</Text>
//             </View>
//             <View style={styles.inputContainer}>
//               <PaperProvider>
//               <TextInput style={styles.input} placeholder="Enter Email" keyboardType="email-address" value={email} onChangeText={setEmail} label="email" mode="outlined"/>
//               </PaperProvider>
//             </View>

//             <View style={styles.inputContainer}>
//               <PaperProvider>
//               <TextInput style={styles.input} placeholder="Enter Password" secureTextEntry value={password} onChangeText={setPassword} label="password" mode="outlined"/>
//               </PaperProvider>
//             </View>

//             <View style={styles.buttonContainer}>
//               <TouchableOpacity onPress={() => handleLogin()}>
//                 <LinearGradient colors={['#5C6BC0', '#7E57C2']} start={[0, 0]} end={[1, 0]} style={styles.button}>
//                   <Text style={styles.buttonText}>Login</Text>
//                 </LinearGradient>
//               </TouchableOpacity>
//             </View>

//             <View style={styles.note}>
//               <TouchableOpacity onPress={goSignUp}>
//                 <Text style={styles.noteText}>
//                   Don't have an account?
//                   <Text style={{ color: '#7E57C2' }}> Sign Up</Text>
//                 </Text>
//               </TouchableOpacity>
//             </View>
//           </View>
//           </ImageBackground>
//         </View>
//     </KeyboardAwareScrollView>
//   );
// }




import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ImageBackground } from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import { useNavigation, StackActions } from '@react-navigation/native';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import Toast from 'react-native-toast-message';
import styles from './ParentLoginStyles';
import AntDesign from '@expo/vector-icons/AntDesign';



export default function ParentLogin() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const { goBack } = useNavigation();
  const navigate = useNavigation();

  function handleLogin() {
    if (email == 'arwa@gmail.com' || email == 'doha@gmail.com') {
      if (password == '1234') {
        navigate.dispatch(
          StackActions.replace('ParentMainTabs', {
            screen: 'ParentStack',
            params: {
              screen: 'ParentHomeScreen',
            },
          })
        );
      }
    } else {
      Toast.show({
        type: 'error',
        text2: 'Invalid email or password',
        text2Style: { color: 'black', fontSize: 16 },
        position: 'top',
      });
    }
  }

  function goSignUp() {
    navigate('ParentStack', { screen: 'ParentSignUp' });
  }

  return (
    <View style={{ flex: 1, backgroundColor: '#fff' }}>
      <KeyboardAwareScrollView
        contentContainerStyle={{ flexGrow: 1 }}
        enableOnAndroid
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.headerContainer}>
          <ImageBackground source={require('../../../../../assets/images/where the sea sleeps mv.jpg')} style={styles.headerBackground}>
          <AntDesign name="arrow-left" size={24} color="white" onPress={goBack} style={styles.backButton}/>
            <Text style={styles.headerTitle}>Login</Text>
          </ImageBackground>
          <View style={styles.curve} />
        </View>

        <View style={styles.formContainer}>
          <View style={styles.inputGroup}>
            <Text style={styles.label}>Email</Text>
            <TextInput
              style={styles.input}
              placeholder="Enter email"
              keyboardType="email-address"
              value={email}
              onChangeText={setEmail}
            />
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Password</Text>
            <TextInput
              style={styles.input}
              placeholder="enter password"
              secureTextEntry
              value={password}
              onChangeText={setPassword}
            />
          </View>

          <TouchableOpacity style={styles.buttonWrapper} onPress={handleLogin}>
            <LinearGradient
              colors={['#231c63', '#2A236F', '#323252']}
              start={{ x: 0, y: 0 }}
              end={{ x: 1, y: 0 }}
              style={styles.loginButton}
            >
              <Text style={styles.buttonText}>LOGIN</Text>
            </LinearGradient>
          </TouchableOpacity>

          <View style={styles.loginContainer}>
            <Text style={styles.loginText}>Don’t have an account? </Text>
            <TouchableOpacity onPress={goSignUp}>
              <Text style={styles.loginLink}>Sign Up</Text>
            </TouchableOpacity>
          </View>
        </View>
      </KeyboardAwareScrollView>
    </View>
  );
}
