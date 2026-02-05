// import { StyleSheet } from 'react-native';

// const styles = StyleSheet.create({
//   container: {
//     flex: 1,
//     paddingBottom: 40,
//     justifyContent:'center',
//     alignItems:'center',
//   },
//   title: {
//     fontSize: 26,
//     color: '#ffffff',
//     fontWeight:'bold',
//     marginTop: 20,
//     textAlign: 'center',
//     marginBottom:20,
//   },
//   form:{
//     borderRadius:20,
//     borderColor:"white",
//     width: '85%',
//     marginTop: 30,
//     backgroundColor: 'rgba(255, 255, 255, 0.45)',
//     justifyContent:'center',
//     alignItems:'center',
//   },
//   inputContainer: {
//     marginBottom: 20,
//     width:'90%',
//   },
//   input: {
//     borderRadius: 10,
//     fontSize: 16,
//     width: '100%',
//     backgroundColor: '#fff',
//   },

//   buttonContainer: {
//     marginTop: 10,
//   },

//   button: {
//     borderRadius: 10,
//     paddingVertical: 12,
//     paddingHorizontal: 50,
//     alignSelf: 'center',
//   },

//   buttonText: {
//     fontSize: 20,
//     fontWeight: 'bold',
//     color: 'white',
//   },

//   note: {
//     width: '100%',
//     marginBlock: 25,
//     alignItems: 'center',
//   },

//   noteText: {
//     color: '#ffffff',
//     fontSize: 18,
//     letterSpacing: 0.4,
//   },

//   arrow: {
//     position: 'absolute',
//     top: 50,
//     left: 20,
//   },

// });

// export default styles;


// import { StyleSheet } from 'react-native';

// const styles = StyleSheet.create({
//   container: {
//     flex: 1,
//     // justifyContent:'flex-end',
//     justifyContent:'center',
//     alignItems:'center',
//   },
//   title: {
//     fontSize: 26,
//     color: '#7E57C2',
//     fontWeight:'bold',
//     marginTop: 20,
//     textAlign: 'center',
//     marginBottom:20,
//   },
//   form:{
//     borderWidth:1,
//     borderRadius:20,
//     borderColor:"white",
//     width: '85%',
//     marginTop: 30,
//     // backgroundColor: 'rgba(255, 255, 255, 0.45)',
//     backgroundColor:"white",
//     justifyContent:'center',
//     alignItems:'center',
//   },
//   inputContainer: {
//     marginBottom: 20,
//     width:'90%',
//   },
//   input: {
//     borderRadius: 10,
//     fontSize: 16,
//     width: '100%',
//     backgroundColor: '#fff',
//   },

//   buttonContainer: {
//     marginTop: 10,
//   },

//   button: {
//     borderRadius: 10,
//     paddingVertical: 12,
//     paddingHorizontal: 50,
//     alignSelf: 'center',
//   },

//   buttonText: {
//     fontSize: 20,
//     fontWeight: 'bold',
//     color: 'white',
//   },

//   note: {
//     width: '100%',
//     marginBlock: 20,
//     alignItems: 'center',
//   },

//   noteText: {
//     color: '#000000',
//     fontSize: 18,
//     letterSpacing: 0.4,
//   },

//   arrow: {
//     position: 'absolute',
//     top: 50,
//     left: 20,
//   },

// });

// export default styles;

import { StyleSheet } from "react-native";

const styles = StyleSheet.create({
  headerContainer: { 
    height: 350, 
    position: 'relative' 
  },
  headerBackground: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  headerTitle: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#fff',
  },
  backButton: {
    position: 'absolute',
    top: 50,
    left: 20 
  },
  curve: {
    position: 'absolute',
    bottom: -40,
    left: 0,
    right: 0,
    height: 90,
    backgroundColor: '#fff',
    borderTopLeftRadius: 70,
    borderTopRightRadius: 70,
  },

  formContainer: {
    flex: 1,
    paddingHorizontal: 32,
    paddingTop:10,
    backgroundColor: '#fff',
  },
  inputGroup: { 
    marginBottom: 25 
  },
  label: {
    fontSize: 15,
    color: '#555',
    marginBottom: 6,
    fontWeight: '600',
  },
  input: {
    borderBottomWidth: 1.5,
    borderBottomColor: '#ddd',
    fontSize: 16,
    paddingVertical: 6,
    color: '#222',
  },
  buttonWrapper: { 
    marginTop: 20, 
    marginBottom: 40
   },
  loginButton: {
    borderRadius: 30,
    paddingVertical: 16,
    alignItems: 'center',
    elevation: 4,
  },
  buttonText: {
    color: '#fff',
    fontSize: 17,
    fontWeight: 'bold',
  },

  loginContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
  },
  loginText: { 
    color: '#666', 
    fontSize: 15 
  },
  loginLink: {
    color: '#1B3B6F',
    fontWeight: 'bold',
    fontSize: 15,
  },
});

export default styles;