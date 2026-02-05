// //behavior={Platform.OS === 'ios' ? 'padding' : 'height'}

// //<KeyboardAvoidingView style={{ flex: 1 }} behavior={Platform.OS === 'ios' ? 'padding' : 'height'}>
// //<ScrollView contentContainerStyle={{ flexGrow: 1 }}>

// {/* <KeyboardAwareScrollView style={{ flex: 1 }}
//         contentContainerStyle={{ flexGrow: 1, justifyContent: 'center' }}
//         enableOnAndroid={true} 
//         extraScrollHeight={10}
//         keyboardShouldPersistTaps="handled"></KeyboardAwareScrollView> */}



import { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ImageBackground, StyleSheet,} from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import { useNavigation } from '@react-navigation/native';
import Toast from 'react-native-toast-message';
import AntDesign from '@expo/vector-icons/AntDesign';
import styles from './styles';


export default function ParentSignUp() {
  const [fName, setFName] = useState('');
  const [lName, setLName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const{goBack}=useNavigation();

  const navigation = useNavigation();


  // async function handleParentSignUp(){
  //   try{
  //     if (!fName || !lName || !email || !phone || !password || !confirmPassword) {
  //       Toast.show({
  //         type: 'error',
  //         text2: 'Please fill all the fields',
  //         text2Style: { color: 'black', fontSize: 16 },
  //         position: 'top',
  //         visibilityTime: 3000,
  //       });
  //       return;
  //     }
  //     const res=await axios.post('https://697603f2c0c36a2a995001f2.mockapi.io/api/v1/users',
  //       {fName,lName,email,phone,password,confirmPassword});

  //       const user = res.data;

  //     if(user){
  //       Toast.show({
  //         type:'success',
  //         text2:`welcome in our family ${fName }${lName}`,
  //         position:'top',
  //         visibilityTime: 3000,
  //         autoHide: true,  
  //       });
  //       navigate("ParentStack",{screen:"ParentHomeScreen",params:{user}});
  //     }
  //   }catch(error){
  //     Toast.show({
  //       type:'error',
  //       text2:"Sign Up failed. Please try again.",
  //       text2Style:{color:'black',fontSize:16},
  //       position:'top',
  //       visibilityTime: 3000,
  //       autoHide: true,  
  //     });
  //   }
  // }

  function handleParentSignUp() {
    if(fName && lName && email && phone && password && confirmPassword){
      // navigation.dispatch(
      //   StackActions.replace("ParentMainTabs", {
      //     screen: "ParentHomeStack",
      //     params: {
      //       screen: "ParentHomeScreen",
      //       params: { fName, lName },
      //     },
      //   })
      // );
      navigation.reset({
        index: 0,
        routes: [
          { 
            name: "ParentMainTabs",
            params: { fName, lName, email, phone }
          },
        ],
      });
    } else {
      Toast.show({
        type:'error',
        text2:"Please fill all the fields",
        text2Style:{color:'black',fontSize:16},
        position:'top',
      });
    }
  }

  function goToLogin() {
    navigation.navigate('ParentStack', { screen: 'ParentLogin' });
  }

  return (
    <View style={{ flex: 1, backgroundColor: '#fff' }}>
      <KeyboardAwareScrollView
        style={{ flex: 1 }}
        contentContainerStyle={{ flexGrow: 1 }}
        enableOnAndroid
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.headerContainer}>
          <ImageBackground source={require('../../../../../assets/images/where the sea sleeps mv.jpg')} style={styles.headerBackground} resizeMode="cover">
              <AntDesign name="arrow-left" size={24} color="white" onPress={goBack} style={styles.backButton}/>
            <Text style={styles.headerTitle}> Sign Up</Text>
          </ImageBackground>

          <View style={styles.curve} />
        </View>

        <View style={styles.formContainer}>
          <View style={styles.inputGroup}>
            <Text style={styles.label}>First Name</Text>
            <View style={styles.inputWrapper}>
              <TextInput
                style={styles.input}
                value={fName}
                onChangeText={setFName}
                placeholder="Enter first name"
              />
            </View>
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Last Name</Text>
            <View style={styles.inputWrapper}>
              <TextInput
                style={styles.input}
                value={lName}
                onChangeText={setLName}
                placeholder="Enter last name"
              />

            </View>
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Phone Number</Text>
            <View style={styles.inputWrapper}>
              <TextInput
                style={styles.input}
                value={phone}
                onChangeText={setPhone}
                keyboardType="phone-pad"
                placeholder="Enter phone number"
              />
            </View>
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Email</Text>
            <View style={styles.inputWrapper}>
              <TextInput
                style={styles.input}
                value={email}
                onChangeText={setEmail}
                keyboardType="email-address"
                autoCapitalize="none"
                placeholder="Enter email"
              />
            </View>
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Password</Text>
            <View style={styles.inputWrapper}>
              <TextInput
                style={styles.input}
                value={password}
                onChangeText={setPassword}
                secureTextEntry
                placeholder='enter password'
              />
            </View>
          </View>

          <View style={styles.inputGroup}>
            <Text style={styles.label}>Confirm Password</Text>
            <View style={styles.inputWrapper}>
              <TextInput
                style={styles.input}
                value={confirmPassword}
                onChangeText={setConfirmPassword}
                secureTextEntry
                placeholder='ReEnter password'
              />
            </View>
          </View>

          <TouchableOpacity style={styles.buttonWrapper} onPress={handleParentSignUp}>
            <LinearGradient
              colors={['#231c63', '#2A236F', '#323252']}
              start={[0,0]}
              end={[1,0]}
              style={styles.signupButton}
            >
              <Text style={styles.buttonText}>SIGN UP</Text>
            </LinearGradient>
          </TouchableOpacity>

          <View style={styles.loginContainer}>
            <Text style={styles.loginText}>Already have an account? </Text>
            <TouchableOpacity onPress={goToLogin}>
              <Text style={styles.loginLink}>Login Here</Text>
            </TouchableOpacity>
          </View>
        </View>
      </KeyboardAwareScrollView>
      </View>
  );
}

