import { StyleSheet } from "react-native";

const styles=StyleSheet.create({
    headerContainer: { 
    height: 250, 
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
  profile: {
    position: 'relative',
    top: 80,
    width: 180,     
    height: 180,
    borderRadius: 90,    
    borderWidth: 2,
    borderColor: '#7745bc',
    backgroundColor: '#f8efff',
    overflow: 'hidden',  
    justifyContent: 'center',
    alignItems: 'center',
    boxShadow: "0px 0px 8px #8e57ee",
    zIndex: 10,
  },

  imgprofile : {
    width: '100%',       
    height: '100%',
    resizeMode: 'contain',  
  },
  btn:{
    borderRadius: 28,
    alignItems: 'center',
    shadowColor: '#8B7AB8',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.3,
    shadowRadius: 16,
    elevation: 10,
    marginTop: 60,
    paddingVertical: 12,
    width:'70%',
    marginInline:'auto',
  },

  btnText: {
    color: '#FFF',
    fontSize: 28,
    fontFamily: 'MouseMemoirs-Regular',
    textAlign: 'center',
    textShadowColor: 'rgba(0, 0, 0, 0.25)',
    textShadowOffset: { width: 0, height: 2 },
    textShadowRadius: 4,
    letterSpacing: 1,
  },
  Intro:{
    marginInline:'auto',
    marginTop:20,
    
  },
  IntroText:{
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 35,
    color: '#452770',
    textAlign:'center',
    textShadowColor: 'rgba(0, 0, 0, 0.25)',
    textShadowOffset: { width: 0, height: 2 },
    textShadowRadius: 4,
  },
  modalBackground: {
    flex: 1,
    backgroundColor: 'rgba(26, 26, 26, 0.5)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  popup: {
    width: '80%',
    backgroundColor: '#2d1b4e',
    borderRadius: 15,
    padding: 15,
    alignItems: 'center',
    marginTop: 20,
    marginInline:'auto'
  },
  popupTitle: {
    color: 'white',
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 30,
    marginBottom: 15,
  },

  grid: {
    width: '95%',
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
  },
  gridItem: {
    width: '48%',    
    aspectRatio: 1,      
    backgroundColor: '#dbcdff', 
    borderRadius: 15,
    marginBottom: 15,
    justifyContent: 'center',
    alignItems: 'center',
  },

  gridImage: {
    width: '70%',
    height: '70%',
    resizeMode: 'contain',
    marginBottom: 6,
  },

  gridText: {
    fontSize: 25,
    textAlign: 'center',
    color: '#452770',
    fontFamily: 'MouseMemoirs-Regular',
  },

  cancelBtn: {
    marginTop: 10,
    padding: 10,
    backgroundColor: '#dbcdff',
    borderRadius: 10,
    width: '50%',
    alignItems: 'center',
  },

  cancelText: {
    color: '#452770',
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 25,
  },
  personality:{
    flexDirection:'row',
    width:'90%',
    borderWidth:1,
    borderRadius:20,
    marginInline:'auto',
    paddingBlock:20,
    borderColor: '#7745bc',
    boxShadow: "0px 0px 8px #8e57ee",
    backgroundColor: '#fbf6ff',
    marginTop:-20,
  },
  imgPerson:{
    width:220,
    height:220,
    marginInlineStart:-40,
    marginBottom:-20,
    marginTop:-20,
    
  },
  personText:{
    flex: 1, 
    marginInlineStart:-20,
    paddingInlineEnd:10
  },
  TextP:{
    color: '#452770',
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 22,
    flexWrap: 'wrap',
    flexShrink: 1,
  },
  advantages: {
    marginTop: 5,
    marginBottom: 20,
    borderRadius: 25,
    // backgroundColor: 'rgba(255, 255, 255, 0.95)',
    // backgroundColor:"#efe6ff",
    borderColor:"#e0cdff",
    borderWidth: 2,
    width:'93%',
    marginInline:'auto',
    marginTop:40,
  },

  advantagesHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },

  advantagesIcon: {
    width: 45,
    height: 45,
    marginRight: 5,
  },

  advTitle: {
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 35,
    color: '#fff',
    flex: 1,
  },

  advSubtitle: {
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 18,
    color: '#9575cd',
    marginBottom: 25,
    fontStyle: 'italic',
  },

  adv: {
    flexDirection: 'column',
    gap: 12,
  },

  advItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#efe9fe',
    paddingVertical: 16,
    paddingHorizontal: 18,
    borderRadius: 20,
    borderWidth: 1,
    borderColor: '#d6bcff',
    shadowColor: '#667eea',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.2,
    shadowRadius: 8,
    elevation: 5,
  },

  advText: {
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 20,
    color: '#452770',
    flex: 1,
    lineHeight: 24,
  },

  funFactsContainer: {
    marginVertical: 15,
    marginBottom: 30,
  },

  funFactTitle: {
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 26,
    color: '#452770',
    textAlign: 'center',
    marginBottom: 12,
  },

  funFact: {
    backgroundColor: '#FFF9E6',
    borderRadius: 20,
    padding: 20,
    borderWidth: 2,
    borderColor: '#FFD700',
  },

  funFactText: {
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 18,
    color: '#667eea',
    textAlign: 'center',
    lineHeight: 24,
  },
  btn2:{
    borderRadius: 28,
    alignItems: 'center',
    shadowColor: '#8B7AB8',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.3,
    shadowRadius: 16,
    elevation: 10,
    paddingVertical: 12,
    width:'70%',
    marginInline:'auto',
    marginBottom:30,
  },
  btnText2:{
    color: '#FFF',
    fontSize: 32,
    fontFamily: 'MouseMemoirs-Regular',
    textAlign: 'center',
    textShadowColor: 'rgba(0, 0, 0, 0.25)',
    textShadowOffset: { width: 0, height: 2 },
    textShadowRadius: 4,
    letterSpacing: 1,
    // marginInlineEnd:10
  }

});

export default styles;