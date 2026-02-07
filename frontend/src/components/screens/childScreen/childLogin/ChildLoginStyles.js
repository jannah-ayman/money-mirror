import { StyleSheet } from 'react-native';

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent:'center',
    alignItems:'center',
  },
  img: {
    width: 180,
    height: 180,
  },
  title: {
    fontSize: 26,
    color: '#ffffff',
    fontWeight:'bold',
    marginTop: 20,
    textAlign: 'center',
    marginBottom:10,
  },
  subtitle:{
    fontSize: 15,
    color: '#ffffff',
    fontWeight:'bold',
    textAlign: 'center',
    marginBottom:20,
  },
  form: {
    borderRadius:20,
    borderColor:"white",
    width: '85%',
    backgroundColor: 'rgba(255, 255, 255, 0.5)',
    justifyContent:'center',
    alignItems:'center',
    marginBottom:150,
  },
  inputContainer: {
    marginBottom: 20,
    width:'90%',
    justifyContent:"center",
    marginTop:10,
  },

  input: {
    borderRadius: 10,
    fontSize: 16,
    width: '100%',
    backgroundColor: '#fff',
  },

  buttonContainer: {
    marginBlock: 20,
    alignItems: 'center',
  },

  button: {
    borderRadius: 12,
    paddingVertical: 14,
    paddingHorizontal: 60,
    alignSelf: 'center',
  },

  buttonText: {
  fontSize: 20,
  fontWeight: 'bold',
  color: '#34495E', 
  textAlign: 'center',
},
  arrow: {
    position: 'absolute',
    top: 45,
    left: 20,
  },

});

export default styles;

