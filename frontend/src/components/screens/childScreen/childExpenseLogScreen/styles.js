import { StyleSheet,Dimensions  } from "react-native";


const { width } = Dimensions.get('window');

const styles = StyleSheet.create({
    headerContainer:{ 
        height: 380,
        position: 'relative'
    },
    headerBackground: {
        flex: 1, 
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
    waveContainer: {
        position: 'absolute',
        bottom: 0,
        left: 0,
        width: '100%',
        height: 20,
    },
    wave: {
        position: 'absolute',
        bottom: 0,
    },
    waveFront: {
        zIndex: 2,
    },
    intro:{
        flexDirection:'row',
        justifyContent:'flex-start',
        alignItems:'center',
        paddingTop:80,
    },
    astr:{
        justifyContent:'center',
        alignItems:'center',
        margin:'auto'
    },
    msgIntro:{
        flex:1,
        borderRadius: 22,
        padding: 10,
        borderWidth: 3,
        borderColor: 'rgba(255, 215, 0, 0.5)',
        backgroundColor: 'rgba(255, 255, 255, 0.8)',
        flexShrink: 1,
        height:'90%',
        marginInlineEnd:20,
        marginBottom:40,
        maxWidth: 320,  
        width: '100%', 
        alignSelf: 'flex-end',
        justifyContent:'center',
    },
    msgText:{
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 23,
        color: '#452770',
        flex: 1,
        lineHeight: 24,
        letterSpacing:0.2
    },
    formContainer: {
        marginHorizontal: 20,
        marginTop: 10,
        marginBottom: 30,
        borderRadius: 30,
        padding: 20,
        backgroundColor: '#FFFFFF',
        borderWidth: 1,
        borderColor: '#E0D4F7',
        // shadowColor: '#5E4B8A',
        // shadowOffset: { width: 0, height: 6 },
        // shadowOpacity: 0.15,
        // shadowRadius: 12,
        // elevation: 8,
        boxShadow: "0px 0px 5px #6e47e4"
    },

    MianTitle: {
        textAlign: 'center',
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 40,
        color: '#452770',
        marginBottom: 8,
        letterSpacing: 0.2,
        textShadowColor: "#452770", 
        textShadowOffset: { width: 0, height: 0 },
        textShadowRadius: 5,
    },

    inputContainer: {
        borderWidth: 1.5,
        borderColor: "#C7B8E8",
        borderRadius: 22,
        paddingVertical: 15,
        paddingHorizontal: 15,
        marginTop: 20,
        width: "100%",
        backgroundColor: '#F9F6FF',
    },

    label: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 25,
        color: '#452770',
        marginBottom: 10,
        textAlign: 'center',
    },
    label2:{
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 25,
        color: '#452770',
        marginBottom: 10,
        textAlign: 'center',
    },

    costInput: {
        textAlign: 'center',
        backgroundColor: '#FFFFFF',
        marginTop: 5,
        fontSize: 25,
        fontFamily: 'MouseMemoirs-Regular',
        color: '#452770',
        borderColor: '#E0D4F7',
        borderWidth: 1.5,
        borderRadius: 18,
        paddingVertical: 12,
        paddingHorizontal: 15,
    },
    ThingInput:{
        textAlign: 'center',
        backgroundColor: '#FFFFFF',
        marginTop: 5,
        fontSize: 25,
        fontFamily: 'MouseMemoirs-Regular',
        color: '#452770',
        borderColor: '#E0D4F7',
        borderWidth: 1.5,
        borderRadius: 18,
        paddingVertical: 12,
        paddingHorizontal: 15,
    },

    section: {
        borderWidth: 1.5,
        borderColor: "#C7B8E8",
        borderRadius: 22,
        paddingVertical: 15,
        paddingHorizontal: 15,
        marginTop: 25,
        width: "100%",
        backgroundColor: '#F9F6FF',
    },

    categoriesGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        justifyContent: 'space-between',
        marginTop: 15,
        gap: 10,
    },

    categoryCard: {
        width: '30%',
        aspectRatio: 1,
        borderWidth: 1.5,
        borderColor: '#D4C5F0',
        borderRadius: 30,
        backgroundColor: '#FFFFFF',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: 12,
        shadowColor: '#8B7AB8',
        shadowOffset: { width: 0, height: 3 },
        shadowOpacity: 0.12,
        shadowRadius: 6,
        elevation: 3,
        padding: 10,
    },

    categoryCardSelected: {
        backgroundColor: '#dbcdff',
        borderWidth: 3,
        borderColor: '#8B7AB8',
        shadowColor: '#452770',
        shadowOffset: { width: 0, height: 5 },
        shadowOpacity: 0.2,
        shadowRadius: 8,
        elevation: 6,
        transform: [{ scale: 1.05 }],
    },

    categoryIcon: {
        width: 40,
        height: 40,
        marginBottom: 5,
        resizeMode: 'contain',
    },

    categoryName: {
        fontSize: 20,
        fontWeight: '600',
        color: '#452770',
        textAlign: 'center',
        fontFamily: 'MouseMemoirs-Regular',
        letterSpacing: 0.3,
    },

    textInput: {
        backgroundColor: '#FFFFFF',
        marginTop: 14,
        fontSize: 20,
        fontFamily: 'MouseMemoirs-Regular',
        color: '#5E4B8A',
        borderColor: '#E0D4F7',
        borderWidth: 1.5,
        borderRadius: 18,
        width: '100%',
        paddingVertical: 12,
        paddingHorizontal: 16,
    },

    placeholderText: {
        fontSize: 18,
        color: '#B5A8D0',
    },

    inputText: {
        fontSize: 24,
        color: '#5E4B8A',
    },

    moodContainer: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        marginTop: 10,
        gap: 10,
    },

    moodChip: {
        // width: (width - 95) / 3,
        width:"30%",
        height: 80,
        borderRadius: 20,
        backgroundColor: '#FFFFFF',
        borderColor: '#FFD580',
        borderWidth: 1.5,
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 12,
        shadowColor: '#E8A87C',
        shadowOffset: { width: 0, height: 3 },
        shadowOpacity: 0.12,
        shadowRadius: 6,
        elevation: 3,
        padding:10,
    },

    moodChipSelected: {
        backgroundColor: '#FFF5E6',
        borderWidth: 2.5,
        borderColor: '#F5A962',
        shadowColor: '#E8A87C',
        shadowOffset: { width: 0, height: 5 },
        shadowOpacity: 0.25,
        shadowRadius: 8,
        elevation: 6,
        transform: [{ scale: 1.05 }],
    },

    moodIcon: {
        width: 40,
        height: 40,
        resizeMode: 'contain',
    },

    btn: {
        borderRadius: 28,
        alignItems: 'center',
        shadowColor: '#8B7AB8',
        shadowOffset: { width: 0, height: 8 },
        shadowOpacity: 0.3,
        shadowRadius: 16,
        elevation: 10,
        marginTop: 28,
        paddingVertical: 12,
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

    
    recentSection: {
        marginHorizontal: 20,
        marginTop: 20,
        marginBottom: 40,
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
        marginBottom:20,
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
        borderLeftColor: '#7B5FC9',
        borderColor: '#E8E1F5',
        shadowColor: '#8B7AB8',
        shadowOffset: { width: 0, height: 3 },
        shadowOpacity: 0.1,
        shadowRadius: 8,
        elevation: 4,
    },

    // purchaseIconContainer: {
    //     width: 62,
    //     height: 62,
    //     borderRadius: 16,
    //     backgroundColor: '#F9F6FF',
    //     justifyContent: 'center',
    //     alignItems: 'center',
    //     marginRight: 14,
    //     borderWidth: 1.5,
    //     borderColor: '#E0D4F7',
    // },

    purchaseIcon: {
        width: 36,
        height: 36,
        resizeMode: 'contain',
        marginInlineEnd:10,
        // borderWidth:1,
        // borderColor:'#8B7AB8',
        // borderRadius:20,
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

    // purchaseMoodContainer: {
    //     width: 52,
    //     height: 52,
    //     borderRadius: 14,
    //     backgroundColor: '#FFF5E6',
    //     justifyContent: 'center',
    //     alignItems: 'center',
    //     borderWidth: 1.5,
    //     borderColor: '#FFD580',
    // },

    // purchaseMoodIcon: {
    //     width: 32,
    //     height: 32,
    //     resizeMode: 'contain',
    // },

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

    // emptyPurchasesIcon: {
    //     width: 70,
    //     height: 70,
    //     opacity: 0.4,
    // },

    // divider: {
    //     height: 1,
    //     backgroundColor: '#E8E1F5',
    //     marginVertical: 6,
    // },

});

export default styles;

