import { StyleSheet, Dimensions } from "react-native";

const { width } = Dimensions.get('window');

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#e2e7fe',
    },
    dashboard: {
        height: 480,
        marginTop: 20,
        borderRadius: 30,
        overflow: 'hidden',
        shadowColor: '#4565f1',
        shadowOffset: { width: 0, height: 10 },
        shadowOpacity: 0.5,
        shadowRadius: 20,
        elevation: 40,
    },

    backImg: {
        resizeMode: "cover",
        height: '100%',
        width: '100%',
    },

    info: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'flex-start',
        paddingHorizontal: 20,
        paddingTop: 15,
    },

    welcomeContainer: {
        flex: 1,
    },

    greeting: {
        color: '#FFF',
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 20,
        opacity: 0.9,
        letterSpacing: 1,
    },

    name: {
        color: '#FFD700',
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 42,
        marginTop: -5,
        textShadowColor: 'rgba(255, 215, 0, 0.5)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 10,
    },

    notiCon: {
        position: 'relative',
        padding: 5,
    },

    notification: {
        width: 55,
        height: 55,
    },

    notiBadge: {
        position: 'absolute',
        top: 0,
        right: 0,
        backgroundColor: '#af97ff',
        borderRadius: 12,
        minWidth: 24,
        height: 24,
        justifyContent: 'center',
        alignItems: 'center',
        borderWidth: 2,
        borderColor: '#FFF',
    },

    notiBadgeText: {
        color: '#FFF',
        fontSize: 12,
        fontWeight: 'bold',
        paddingHorizontal: 6,
    },

    // Nova Character Section
    novaContainer: {
        alignItems: 'center',
        marginTop: -10,
    },

    nova: {
        width: 180,
        height: 180,
        marginBottom: -20,
    },

    speechBubble: {
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        borderRadius: 20,
        padding: 10,
        marginHorizontal: 20,
        marginBottom: 15,
        marginTop:5,
        borderWidth: 2,
        borderColor: '#FFD700',
        shadowColor: '#FFD700',
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.3,
        shadowRadius: 8,
        elevation: 8,
    },

    description: {
        color: '#ffffff',
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 26,
        textAlign: 'center',
        lineHeight: 26,
        marginBlock:20,
    },

    // Balance Card
    balance: {
        marginHorizontal: 20,
        borderRadius: 25,
        padding: 3,
        marginBottom: 20,
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.4,
        shadowRadius: 12,
        elevation: 10,
    },

    balanceContent: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-around',
        borderRadius: 22,
        padding: 10,
        borderWidth: 2,
        borderColor: 'rgba(255, 215, 0, 0.5)',
    },

    balanceIcon: {
        width: 40,
        height: 40,
    },

    balanceTitle: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 25,
        color: '#452770',
        textAlign: 'center',
        marginBottom: -5,
    },

    balanceAmount: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 55,
        color: '#452770',
        textAlign: 'center',
        // textShadowColor: 'rgba(255, 107, 157, 0.3)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 8,
        marginTop:10,
    },

    // Action Buttons
    btnContainer: {
        marginVertical: 25,
        flexDirection: 'row',
        justifyContent: 'center',
        gap: 15,
        paddingHorizontal: 10,
    },

    actionBtn: {
        flex: 1,
        maxWidth: width * 0.44,
    },

    btnGradient: {
        borderRadius: 25,
        padding: 20,
        alignItems: 'center',
        shadowColor: '#7B68EE',
        shadowOffset: { width: 0, height: 8 },
        shadowOpacity: 0.4,
        shadowRadius: 15,
        elevation: 12,
        minHeight: 160,
        justifyContent: 'center',
    },

    btnIcon: {
        width: 60,
        height: 60,
        marginBottom: 10,
    },

    btnText: {
        color: '#FFF',
        fontSize: 26,
        fontFamily: 'MouseMemoirs-Regular',
        textAlign: 'center',
        marginTop: 8,
        textShadowColor: 'rgba(0, 0, 0, 0.3)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 4,
    },

    btnSubtext: {
        color: 'rgba(255, 255, 255, 0.9)',
        fontSize: 16,
        fontFamily: 'MouseMemoirs-Regular',
        marginTop: 4,
    },

    // Mission Card
    cardContainer: {
        marginVertical: 10,
    },

    missionCard: {
        borderRadius: 25,
        overflow: 'hidden',
        shadowColor: '#667eea',
        shadowOffset: { width: 0, height: 10 },
        shadowOpacity: 0.4,
        shadowRadius: 20,
        elevation: 15,
    },

    cardBackground: {
        width: '100%',
        height: 280,
    },

    cardOverlay: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 25,
    },

    cardTitle: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 40,
        color: '#FFF',
        marginBottom: 15,
        textShadowColor: 'rgba(0, 0, 0, 0.5)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 8,
    },

    cardText: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 25,
        color: '#FFF',
        textAlign: 'center',
        lineHeight: 28,
        textShadowColor: 'rgba(0, 0, 0, 0.3)',
        textShadowOffset: { width: 0, height: 1 },
        textShadowRadius: 4,
    },
    card2Title:{
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 35,
        marginBottom:100,
        color: '#FFF',
        textShadowColor: 'rgba(0, 0, 0, 0.5)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 8,
    },
    card2Text: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 20,
        color: '#FFF',
        textAlign: 'center',
        lineHeight: 28,
        textShadowColor: 'rgba(0, 0, 0, 0.3)',
        textShadowOffset: { width: 0, height: 1 },
        textShadowRadius: 4,
        marginTop:20
    },

    // Personality Insights
    advantages: {
        marginTop: 5,
        marginBottom: 20,
        borderRadius: 25,
        // backgroundColor: 'rgba(255, 255, 255, 0.95)',
        // backgroundColor:"#efe6ff",
        borderColor:"#e0cdff",
        borderWidth: 2,
        // borderColor: '#FFD700',
        // shadowColor: '#FFD700',
        // shadowOffset: { width: 0, height: 8 },
        // shadowOpacity: 0.3,
        // shadowRadius: 15,
        // elevation: 10,
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

    // Fun Facts Section
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

    // Modal Styles
    modalOverlay: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 20,
        backgroundColor: 'rgba(51, 51, 51, 0.7)',
    },

    modalContent: {
        width: '90%',
        minHeight:400,
        borderRadius: 30,
        padding: 25,
        shadowOffset: { width: 0, height: 10 },
        shadowOpacity: 0.5,
        shadowRadius: 25,
        elevation: 20,
        borderWidth: 0.5,
    },
    modalHeader: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: 20,
    },
    modalIcon: {
        width: 30,
        height: 30,
        marginRight: 10,
    },

    modalTitle: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 32,
        color: '#FFF',
        textShadowColor: 'rgba(0, 0, 0, 0.3)',
        textShadowOffset: { width: 0, height: 2 },
        textShadowRadius: 4,
    },

    modalScroll: {
        flex: 1,
    },

    notificationContainer: {
        paddingBottom: 10,
    },

    noti: {
        marginBottom: 15,
    },

    msgContainer: {
        backgroundColor: 'rgba(255, 255, 255, 0.95)',
        paddingVertical: 10,
        paddingHorizontal: 12,
        borderRadius: 20,
        borderWidth: 2,
        borderColor: '#FFD700',
        flexDirection: 'row',
        alignItems: 'center',
    },

    msg: {
        fontSize: 22,
        color: '#452770',
        flex: 1,
        lineHeight: 24,
        fontFamily: 'MouseMemoirs-Regular',
    },

    emptyState: {
        padding: 40,
        alignItems: 'center',
        justifyContent: 'center',
    },

    emptyStateText: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 30,
        color: '#FFF',
        textAlign: 'center',
        opacity: 0.8,
    },

    closeButton: {
        marginTop: 20,
        backgroundColor: 'rgba(255, 255, 255, 0.2)',
        paddingVertical: 14,
        paddingHorizontal: 30,
        borderRadius: 25,
        borderWidth: 2,
        borderColor: '#FFD700',
        alignItems: 'center',
    },

    closeText: {
        fontFamily: 'MouseMemoirs-Regular',
        fontSize: 28,
        color: '#FFF',
    },

    glowWrapper: {
        borderRadius: 20,
        shadowColor: '#6227c9',
        shadowOffset: { width: 0, height: 0 },
        shadowOpacity: 1,
        shadowRadius: 25,
        elevation: 15,
        marginVertical: 10,
    },
});

export default styles;