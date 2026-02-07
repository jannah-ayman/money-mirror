import { StyleSheet } from "react-native";

const styles=StyleSheet.create({
  headerContainer: {
    width: '100%',
    height: 320,
    paddingTop:10,
    borderBottomLeftRadius: 60,
    borderBottomRightRadius: 60,
    overflow: 'hidden',
  },
  backButton: {
    position: 'absolute',
    top: 50,
    left: 25,
  },
  mainHeader:{
    flexDirection:'row',
    margin:'auto',
  },
  img1: {
    width: 220,
    height: 220,
    resizeMode: 'contain',
    marginTop:30,
    marginInlineStart:-20
  },
  img2: {
    width: 250,
    height: 250,
    resizeMode: 'contain',
    marginInlineStart:-50,
    marginTop:-30,
  },
  FilterCard: {
    // backgroundColor: '#ebe7ec',
    backgroundColor:'#F9F6FF',
    padding: 16,
    borderRadius: 12,
    borderWidth: 0.5,
    borderColor: '#C7B8E8',
    width:'85%',
    position:'relative',
    top:-30,
    marginInline:'auto',
    boxShadow: "0px 0px 8px #4B2C82"
  },
  img3:{
    width:25,
    height:25,
    marginInlineEnd:10,
  },
  title: {
    textAlign: 'center',
    fontFamily: 'MouseMemoirs-Regular',
    fontSize: 30,
    color: '#452770',
    marginBottom:15,
  },
  row: {
    flexDirection: 'row',
    gap: 15,
  },
  dropdown: {
    flex: 1,
    backgroundColor: '#ffffff',
    borderRadius: 8,
    borderWidth: 1,
    // borderColor: '#891aa5',
    borderColor: '#C7B8E8',
    overflow: 'hidden',
  },
  container: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginVertical: 0,
    width:'95%',
    margin:'auto',
    marginBlock:10
  },
  card: {
    flex: 1,
    margin: 5,
    borderRadius: 15,
    alignItems: 'center',
    paddingInline:10,
    paddingBlock:10
  },
  cardTitle: {
    color: 'white',
    fontSize: 25,
    fontFamily: 'MouseMemoirs-Regular',
  },
  cardValue: {
    color: 'white',
    fontSize: 35,
    fontFamily: 'MouseMemoirs-Regular',
    marginTop: 5,
  },
  recentSection: {
        marginHorizontal: 20,
        marginTop: 20,
        marginBottom: 100,
        borderRadius: 30,
        // backgroundColor: '#dbcdff',
        borderWidth: 0.1,
        // borderColor: '#8B7AB8',
        shadowColor: '#4f3789',
        shadowOffset: { width: 0, height: 6 },
        shadowOpacity: 0.15,
        shadowRadius: 12,
        elevation: 8,

    },
    recentTitle: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 30,
        // color: '#452770',
        color: '#fff',
        letterSpacing: 1,
        textAlign: 'center',
        marginBottom:10,
    },

    starIconLeft: {
        position: 'absolute',
        left: 20,
        fontSize: 28,
    },

    starIconRight: {
        position: 'absolute',
        right: 20,
        fontSize: 28,
    },

    purchasesListContainer: {
        padding: 22,
    },

    purchaseCard: {
        flexDirection: 'row',
        justifyContent:'space-between',
        alignItems: 'center',
        backgroundColor: '#FDFCFF',
        borderRadius: 22,
        padding: 8,
        marginBottom: 14,
        borderWidth: 1.5,
        borderLeftWidth: 4,
        borderLeftColor: '#bda5ff',
        borderColor: '#E8E1F5',
        shadowColor: '#8B7AB8',
        shadowOffset: { width: 0, height: 3 },
        shadowOpacity: 0.1,
        shadowRadius: 8,
        elevation: 4,
    },

    purchaseIcon: {
        width: 36,
        height: 36,
        resizeMode: 'contain',
        marginInlineEnd:10,
    },

    purchaseName: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 24,
        color: '#452770',
        marginBottom: 4,
        letterSpacing: 0.3,
    },

    purchaseCost: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 20,
        color: '#F5A962',
        fontWeight: 'bold',
    },

    purchaseDate: {
      fontFamily: 'MouseMemoirs-Regular',
      fontSize: 18,
      color: '#8B7AB8',
    },

    emptyPurchasesContainer: {
        padding:20,
        alignItems: 'center',
        justifyContent: 'center',
    },

    emptyPurchasesText: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 25,
        color: '#ffffff',
        textAlign: 'center',
    },

});

export default styles;