// import { StyleSheet } from 'react-native';

// const styles = StyleSheet.create({
//   container: {
//     flex: 1,
//     justifyContent:"center",
//     alignItems:"center"
//   },
//   title: {
//     fontSize: 25,
//     color: '#ffffff',
//     textAlign: 'center',
//     fontWeight: 'bold',
//     marginBlock: 20,
//   },
//   form: {
//     borderRadius:20,
//     borderColor:"white",
//     width: '85%',
//     justifyContent:'center',
//     alignItems: 'center',
//     marginTop: 30,
//     backgroundColor: 'rgba(255, 255, 255, 0.45)',
//   },
//   inputContainer: {
//     marginBottom: 20,
//     width:'90%',
//   },
//   input: {
//     borderRadius: 10,
//     fontSize: 16,
//     width: '100%',
//   },
//   buttonContainer:{
//     marginTop:20,
//   },
//   button:{
//     borderRadius: 10,
//     paddingVertical: 10,
//     paddingHorizontal: 40,
//     alignSelf: 'center',
//   },
//   buttonText:{
//     fontSize: 20,
//     fontWeight: 'bold',
//     color: '#ffffff',
//   },
//   note:{
//     width: '100%',
//     marginBlock: 30,
//     alignItems: 'center',
//   },
//   noteText:{
//     color: 'white',
//     fontSize: 17,
//     letterSpacing: 0.5,
//   },
//   arrow:{
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
//     justifyContent:"flex-end",
//     alignItems:"center"
//   },
//   title: {
//     fontSize: 25,
//     color: '#7E57C2',
//     textAlign: 'center',
//     fontWeight: 'bold',
//     marginBlock: 20,
//   },
//   form: {
//     borderWidth:1,
//     borderRadius:20,
//     borderColor:"white",
//     width: '85%',
//     justifyContent:'center',
//     alignItems: 'center',
//     marginBottom: 100,
//     // backgroundColor: 'rgba(255, 255, 255, 0.45)',
//     backgroundColor:"white",
//   },
//   inputContainer: {
//     marginBottom: 10,
//     width:'90%',
//   },
//   input: {
//     borderRadius: 10,
//     fontSize: 16,
//     width: '100%',
//   },
//   buttonContainer:{
//     marginTop:20,
//   },
//   button:{
//     borderRadius: 10,
//     paddingVertical: 10,
//     paddingHorizontal: 40,
//     alignSelf: 'center',
//   },
//   buttonText:{
//     fontSize: 20,
//     fontWeight: 'bold',
//     color: '#ffffff',
//   },
//   note:{
//     width: '100%',
//     marginBlock: 30,
//     alignItems: 'center',
//   },
//   noteText:{
//     color: 'black',
//     fontSize: 17,
//     letterSpacing: 0.5,
//   },
//   arrow:{
//     position: 'absolute',
//     top: 50,
//     left: 20,
//   },
// });

// export default styles;




import { StyleSheet } from "react-native";

const styles = StyleSheet.create({
  headerContainer:{ 
    height: 250,
    position: 'relative'
    },
  headerBackground: {
     flex: 1, 
     justifyContent: 'center', 
     alignItems: 'center' 
    },
  backButton: {
    position: 'absolute',
    top: 50,
    left: 20 
  },
  headerTitle: {
    fontSize: 32,
    fontWeight: 'bold',
    color: '#fff',
    textAlign: 'center',
  },
  curve: {
    position: 'absolute',
    bottom: -30,
    left: 0,
    right: 0,
    height: 80,
    backgroundColor: '#fff',
    borderTopLeftRadius: 70,
    borderTopRightRadius: 70,
  },
  formContainer: {
    flex: 1,
    paddingHorizontal: 30,
    backgroundColor: '#fff',
  },
  inputGroup: { 
    marginBottom: 20 
  },
  label: {
    fontSize: 15,
    color: '#555',
    marginBottom: 5,
    fontWeight: '600',
  },
  inputWrapper: {
    flexDirection: 'row',
    alignItems: 'center',
    borderBottomWidth: 1.5,
    borderBottomColor: '#ddd',
    paddingBottom: 6,
  },
  input: {
    fontSize: 16,
    color: '#222',
    paddingVertical: 4,
  },
  buttonWrapper: { 
    marginTop: 20, 
    marginBottom: 16 
  },
  signupButton: {
    borderRadius: 30,
    paddingVertical: 16,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOpacity: 0.25,
    shadowRadius: 5,
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
    marginTop: 8,
  },
  loginText: { 
    color: '#666', 
    fontSize: 15
   },
  loginLink: {
    color: '#214a8b',
    fontWeight: 'bold',
    fontSize: 15,
  },
});

export default styles;