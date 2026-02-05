import { View, Text, TouchableOpacity, Image, Modal, ImageBackground } from 'react-native'
import { SafeAreaView } from 'react-native-safe-area-context'
import styles from './styles'
import { useNavigation, useRoute } from '@react-navigation/native'
import { ScrollView } from 'react-native-gesture-handler';
import { useEffect, useState } from 'react';
import { LinearGradient } from 'expo-linear-gradient';
import { Pressable } from 'react-native';
import { UseChildStore } from '../../../../store/UseChildStore';


export default function ChildHomeScreen() {

  const setCurrentChild =UseChildStore(state=>state.setCurrentChild);

  const [modalVisible, setModalVisible] = useState(false);

  const navigation=useNavigation();

  const route = useRoute();
  const child = route.params?.child ?? {};  
  const {name, balance, notification, insights} = child;


  function addPurchase(){
    navigation.navigate("childExpenseLogScreen",{child});
  }

  useEffect(() => {
    if (child) {
      setCurrentChild(child);
    }
  }, [child]);


  function showAdv(insights = []) {
    return (
      <View style={styles.adv}>
        {insights.map(item => (
          <View style={styles.advItem} key={item.id}>
            <Image
              source={require("../../../../../assets/images/star (3).png")}
              style={{ width: 28, height: 28, marginRight: 8 }}
            />
            <Text style={styles.advText}>{item.adv}</Text>
          </View>
        ))}
      </View>
    );
  }

  function showNotification(notification = []) {
    return (
      <View style={styles.notificationContainer}>
        {notification.map(item => (
          <View style={styles.noti} key={item.id}>
            <View style={styles.msgContainer}>
              <Image
                source={require("../../../../../assets/images/check-mark.png")}
                style={{ width: 40, height: 40, marginRight: 12 }}
              />
              <Text style={styles.msg}>{item.msg}</Text>
            </View>
          </View>
        ))}
      </View>
    );
  }

  useEffect(() => {
    showAdv(insights)
  }, []);

  return (
    <LinearGradient
    colors={['#F4F6FF', '#EEF1FF', '#E9EDFF']}
    style={{ flex: 1 }}
    >
    <SafeAreaView style={styles.container}>
      <ScrollView 
        style={{ flex: 1 }}
        contentContainerStyle={{ paddingBottom: 100, paddingHorizontal: 15 }}
        showsVerticalScrollIndicator={false}
        scrollEventThrottle={2}
      >
        {/* Space Dashboard Header */}
        <View style={styles.dashboard}>
          <ImageBackground 
            source={require("../../../../../assets/images/download (45).jpg")} 
            style={styles.backImg}
          >
            <View style={styles.info}>
              <View style={styles.welcomeContainer}>
                <Text style={styles.greeting}>Hello Space Explorer!</Text>
                <Text style={styles.name}>{name}</Text>
              </View>
              <TouchableOpacity 
                onPress={() => setModalVisible(true)} 
                style={styles.notiCon}
              >
                <Image 
                  source={require("../../../../../assets/images/notification (1)_transparent.gif")} 
                  style={styles.notification}
                />
                {notification?.length > 0 && (
                  <View style={styles.notiBadge}>
                    <Text style={styles.notiBadgeText}>{notification.length}</Text>
                  </View>
                )}
              </TouchableOpacity>
            </View>

            {/* Nova Character */}
            <View style={styles.novaContainer}>
              <Image 
                source={require('../../../../../assets/images/cute-astronaut-peace-hand-cartoon-vector-icon-illustration-science-technology-icon-isolated-flat.png')} 
                style={styles.nova}
              />
              {/* <View style={styles.speechBubble}> */}
                <Text style={styles.description}>
                  Hi there, friend! I'm Nova, your space money buddy!
                </Text>
              {/* </View> */}
            </View>

            {/* Balance Card */}
            <LinearGradient
              colors={['rgba(255,255,255,0.9)', 'rgba(255,255,255,0.7)']}
              style={styles.balance}
              start={{ x: 0, y: 0 }}
              end={{ x: 1, y: 1 }}
            >
              <View style={styles.balanceContent}>
                <Image
                  source={require("../../../../../assets/images/star (3).png")}
                  style={styles.balanceIcon}
                />
                <View>
                  <Text style={styles.balanceTitle}>Your Galaxy Credits</Text>
                  <Text style={styles.balanceAmount}>$ {balance}</Text>
                </View>
                <Image
                  source={require("../../../../../assets/images/rocket (1).png")}
                  style={styles.balanceIcon}
                />
              </View>
            </LinearGradient>
          </ImageBackground>
        </View>

        {/* Action Buttons */}
        <View style={styles.btnContainer}>
          <Pressable
            style={styles.actionBtn}
            android_ripple={null}
            onPress={()=>addPurchase()}
          >
            {({ pressed }) => (
              <LinearGradient
                colors={['#4B2C8C', '#3552B3']}
                // colors={['#667eea', '#764ba2']}
                style={[
                  styles.btnGradient,
                  pressed && { transform: [{ scale: 0.97 }] }
                ]}
                start={{ x: 0, y: 0 }}
                end={{ x: 1, y: 1 }}
              >
                <Image
                  source={require("../../../../../assets/images/order-processing.png")}
                  style={styles.btnIcon}
                />
                <Text style={styles.btnText}>Add Purchase</Text>
              </LinearGradient>
            )}
          </Pressable>

          <Pressable
            style={styles.actionBtn}
            android_ripple={null}
            onPress={() => {
            }}
          >
            {({ pressed }) => (
              <LinearGradient
                colors={['#4B2C8C', '#3552B3']}
                style={[
                  styles.btnGradient,
                  pressed && { transform: [{ scale: 0.97 }] }
                ]}
                start={{ x: 0, y: 0 }}
                end={{ x: 1, y: 1 }}
              >
                <Image
                  source={require("../../../../../assets/images/target.png")}
                  style={styles.btnIcon}
                />
                <Text style={styles.btnText}>My Goals</Text>
              </LinearGradient>
            )}
          </Pressable>

        </View>

        {/* insights Insights */}
        <View style={styles.advantages}>
        <LinearGradient
        colors={['#4B2C8C', '#3552B3']}
        style={{borderRadius:20,padding:20}}
        >

          <View style={styles.advantagesHeader}>
            <Image
              source={require("../../../../../assets/images/starss.gif")}
              style={styles.advantagesIcon}
            />
            <Text style={styles.advTitle}>Your Space Powers!</Text>
          </View>
          {/* <Text style={styles.advSubtitle}>
            Discover what makes you a smart space explorer:
          </Text> */}
          {showAdv(insights)}
        </LinearGradient>
        </View>



        {/* Mission Card */}
        <View style={styles.cardContainer}>
          <LinearGradient
            colors={['#667eea', '#764ba2']}
            style={styles.missionCard}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
          >
            <ImageBackground
              source={require("../../../../../assets/images/s2.gif")}
              style={styles.cardBackground}
              imageStyle={{ borderRadius: 25 }}
            >
              <View style={styles.cardOverlay}>
                <Text style={styles.cardTitle}>Your Mission</Text>
                <Text style={styles.cardText}>
                  Ready for an adventure with Nova? {"\n"}
                  Explore the galaxy of smart spending, {"\n"}
                  collect stars, and reach for the moon! 
                </Text>
              </View>
            </ImageBackground>
          </LinearGradient>
        </View>


        
        {/* Mission Card 2 */}
        {/* <View style={styles.cardContainer}>
          <LinearGradient
            colors={['#667eea', '#764ba2']}
            style={styles.missionCard}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
          >
            <ImageBackground
              source={require("../../../../../assets/images/download (13).jpg")}
              style={styles.cardBackground}
              imageStyle={{ borderRadius: 25 }}
            >
              <View style={styles.cardOverlay}>
                <Text style={styles.card2Title}>Ready to Explore Your Galaxy?</Text>
                <Text style={styles.card2Text}>
                  Your galaxy is full of stations! Hop from one to another, discover, learn, and unlock all the cosmic treasures waiting for you!"
                </Text>
              </View>
            </ImageBackground>
          </LinearGradient>
        </View> */}

        
        
        {/* Fun Facts Section */}
        {/* <View style={styles.funFactsContainer}>
          <Text style={styles.funFactTitle}>💫 Did You Know?</Text>
          <View style={styles.funFact}>
            <Text style={styles.funFactText}>
              Every dollar you save is like collecting a star! ⭐
              Keep exploring and learning about money!
            </Text>
          </View>
        </View> */}
      </ScrollView>

      {/* Notifications Modal */}
      <Modal
        animationType="slide"
        visible={modalVisible}
        transparent={true}
        onRequestClose={() => setModalVisible(false)}
        statusBarTranslucent={true}
      >
        <View style={styles.modalOverlay}>
          <LinearGradient
            colors={[ '#4B2C82', '#7B5FC9']}
            style={styles.modalContent}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
          >
            <View style={styles.modalHeader}>
              <Image
                source={require("../../../../../assets/images/satellite.png")}
                style={styles.modalIcon}
              />
              <Text style={styles.modalTitle}>Space Messages</Text>
            </View>

            <ScrollView 
              showsVerticalScrollIndicator={false}
              style={styles.modalScroll}
            >
              {notification?.length > 0 ? (
                showNotification(notification)
              ) : (
                <View style={styles.emptyState}>
                  <Text style={styles.emptyStateText}>
                    No messages from the galaxy yet! 
                  </Text>
                </View>
              )}
            </ScrollView>

            <TouchableOpacity 
              onPress={() => setModalVisible(false)}
              style={styles.closeButton}
            >
              <Text style={styles.closeText}>Close Mission Control</Text>
            </TouchableOpacity>
          </LinearGradient>
        </View>
      </Modal>    
    </SafeAreaView>
    </LinearGradient>
  )
}