import React from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Image,
  StatusBar,
  Alert
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function ParentProfileScreen() {
  const navigation = useNavigation()

  const handleLogout = () => {
    Alert.alert(
      'Logout',
      'Are you sure you want to logout?',
      [
        {
          text: 'Cancel',
          style: 'cancel'
        },
        {
          text: 'Logout',
          onPress: () => {
            console.log('User logged out - ParentProfileScreen.js:32')
            // navigation.navigate('Login')
          },
          style: 'destructive'
        }
      ]
    )
  }

  const menuItems = [
    {
      id: '1',
      icon: 'pencil',
      title: 'Edit Profile Details ',
      color: '#FF6B9D',
      onPress: () => {
        console.log('Edit Profile Details - ParentProfileScreen.js:48')
        navigation.navigate('ParentHomeStack', {
        screen: 'EditProfileScreen',
});
      }
    },
    {
      id: '2',
      icon: 'people',
      title: 'Manage Children',
      color: '#5F3BFF',
       onPress: () => {
        console.log('Manage Children - ParentProfileScreen.js:60')
        navigation.navigate('ParentHomeStack', {
        screen: 'ManageChildren', 
  })}
    },
    {
      id: '3',
      icon: 'lock-closed',
      title: 'Change Password',
      color: '#2A236F',
      onPress: () => {
        console.log('Change Password - ParentProfileScreen.js:71')
         navigation.navigate('ParentHomeStack', {
          screen: 'ChangePasswordScreen',
});      }
    },
    {
      id: '4',
      icon: 'mail',
      title: 'Change Email Address',
      color: '#2A236F',
      onPress: () => {
        console.log('Change Email Address - ParentProfileScreen.js:82')
       navigation.navigate('ParentHomeStack', {
       screen: 'ChangeEmailScreen',
});      }
    },
    {
      id: '5',
      icon: 'notifications',
      title: 'Notification Settings',
      color: '#2A236F',
      onPress: () => {
        console.log('Notification Settings - ParentProfileScreen.js:93')
        navigation.navigate('ParentHomeStack', {
           screen: 'NotificationSettingsScreen',
});
      }
    },
    {
      id: '6',
      icon: 'settings',
      title: 'App Settings',
      color: '#2A236F',
      onPress: () => {
        console.log('App Settings - ParentProfileScreen.js:105')
        navigation.navigate('ParentHomeStack', {
           screen: 'AppSettingsScreen',
});
      }
    },
    {
      id: '7',
      icon: 'help-circle',
      title: 'Help & Support',
      color: '#2A236F',
      onPress: () => {
        console.log('Help & Support - ParentProfileScreen.js:117')
        navigation.navigate('ParentHomeStack', {
           screen: 'HelpSupportScreen',
});
      }
    },
    {
      id: '8',
      icon: 'log-out',
      title: 'Logout',
      color: '#FF6B9D',
      onPress: handleLogout
    },
  ]

  return (
    <View style={styles.container}>
      <StatusBar barStyle="light-content" />

      {/* Header with Gradient */}
      <LinearGradient
        colors={['#1F1147', '#2B1055', '#4B0082', '#5F3BFF']}
        start={{ x: 0, y: 0 }}
        end={{ x: 1, y: 1 }}
        style={styles.header}
      >
        {/* Back Button */}
        <TouchableOpacity 
          style={styles.backButton}
          onPress={() => navigation.goBack()}
        >
          <Ionicons name="arrow-back" size={24} color="#fff" />
        </TouchableOpacity>

        {/* Profile Info */}
        <View style={styles.profileContainer}>
          <View style={styles.avatarContainer}>
            <Image 
              source={require('../../../../../assets/images/parent-avatar.png')} 
              style={styles.avatar}
              defaultSource={require('../../../../../assets/images/default-avatar.png')}
            />
            <TouchableOpacity style={styles.editAvatarButton}>
              <LinearGradient
                colors={['#FFFFFF', '#F0F0F0']}
                style={styles.editAvatarGradient}
              >
                <Ionicons name="camera" size={16} color="#5F3BFF" />
              </LinearGradient>
            </TouchableOpacity>
          </View>
          
          <Text style={styles.profileName}>Jenifer Anderson</Text>
          <Text style={styles.profileEmail}>jenifer.anderson@email.com</Text>
        </View>
      </LinearGradient>

      {/* Menu Items */}
      <ScrollView 
        style={styles.menuContainer}
        showsVerticalScrollIndicator={false}
        contentContainerStyle={styles.menuContent}
      >
        <View style={styles.menuCard}>
          {menuItems.map((item, index) => (
            <React.Fragment key={item.id}>
              <TouchableOpacity 
                style={styles.menuItem}
                onPress={item.onPress}
                activeOpacity={0.7}
              >
                <View style={styles.menuLeft}>
                  <View style={[
                    styles.iconContainer,
                    { backgroundColor: `${item.color}15` }
                  ]}>
                    <Ionicons 
                      name={item.icon} 
                      size={20} 
                      color={item.color} 
                    />
                  </View>
                  <Text style={[
                    styles.menuText,
                    item.id === '8' && styles.logoutText
                  ]}>
                    {item.title}
                  </Text>
                </View>
                
                <Ionicons 
                  name="chevron-forward" 
                  size={20} 
                  color="#C0C0D0" 
                />
              </TouchableOpacity>

              {index < menuItems.length - 1 && (
                <View style={styles.divider} />
              )}
            </React.Fragment>
          ))}
        </View>

        {/* App Info Section */}
        <View style={styles.appInfoSection}>
          <Text style={styles.appInfoTitle}>Money Mirror</Text>
          <Text style={styles.versionText}>Version 1.0.0</Text>
          <Text style={styles.copyrightText}>© 2026 Money Mirror. All rights reserved.</Text>
        </View>
      </ScrollView>

      {/* Bottom Navigation */}
      <View style={styles.bottomNav}>
        <TouchableOpacity 
          style={styles.navItem}
          onPress={() => {
            console.log('Navigate to Home - ParentProfileScreen.js:234')
            navigation.navigate('ParentHome')
          }}
        >
          <Ionicons name="home" size={24} color="#C0C0D0" />
        </TouchableOpacity>

        <TouchableOpacity 
          style={styles.navItem}
          onPress={() => console.log('Navigate to Reports - ParentProfileScreen.js:243')}
        >
          <Ionicons name="stats-chart" size={24} color="#C0C0D0" />
        </TouchableOpacity>

        <TouchableOpacity 
          style={styles.navItem}
          onPress={() => console.log('Navigate to Notifications - ParentProfileScreen.js:250')}
        >
          <Ionicons name="notifications" size={24} color="#C0C0D0" />
        </TouchableOpacity>

        <TouchableOpacity style={styles.navItem}>
          <Ionicons name="person" size={24} color="#5F3BFF" />
          <View style={styles.activeIndicator} />
        </TouchableOpacity>
      </View>
    </View>
  )
}

const styles = StyleSheet.create({
  /* ================= BASE ================= */
  container: {
    flex: 1,
    backgroundColor: '#F4F6FB',
  },

  /* ================= HEADER ================= */
  header: {
    paddingTop: StatusBar.currentHeight ? StatusBar.currentHeight + 15 : 55,
    paddingBottom: 40,
    borderBottomLeftRadius: 30,
    borderBottomRightRadius: 30,
    shadowColor: '#3B1DFF',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.7,
    shadowRadius: 25,
    elevation: 10,
  },

  backButton: {
    marginLeft: 24,
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 20,
  },

  /* ================= PROFILE ================= */
  profileContainer: {
    alignItems: 'center',
    paddingHorizontal: 24,
  },

  avatarContainer: {
    position: 'relative',
    marginBottom: 16,
  },

  avatar: {
    width: 100,
    height: 100,
    borderRadius: 50,
    borderWidth: 4,
    borderColor: '#FFFFFF',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
    elevation: 8,
    backgroundColor: '#E0E0E0',
  },

  editAvatarButton: {
    position: 'absolute',
    bottom: 0,
    right: 0,
    borderRadius: 15,
    overflow: 'hidden',
  },

  editAvatarGradient: {
    width: 30,
    height: 30,
    borderRadius: 15,
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 2,
    borderColor: '#FFFFFF',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 4,
  },

  profileName: {
    color: '#FFFFFF',
    fontSize: 24,
    fontWeight: '800',
    letterSpacing: 0.5,
    marginBottom: 4,
  },

  profileEmail: {
    color: 'rgba(255, 255, 255, 0.8)',
    fontSize: 14,
    fontWeight: '500',
  },

  /* ================= MENU ================= */
  menuContainer: {
    flex: 1,
    marginTop: -20,
  },

  menuContent: {
    paddingHorizontal: 24,
    paddingBottom: 120,
  },

  menuCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 24,
    paddingVertical: 8,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.08,
    shadowRadius: 12,
    elevation: 4,
    marginBottom: 20,
  },

  menuItem: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 16,
    paddingHorizontal: 20,
  },

  menuLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },

  iconContainer: {
    width: 40,
    height: 40,
    borderRadius: 12,
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 16,
  },

  menuText: {
    color: '#231c63',
    fontSize: 16,
    fontWeight: '600',
    flex: 1,
  },

  logoutText: {
    color: '#FF6B9D',
  },

  divider: {
    height: 1,
    backgroundColor: '#F0F0F5',
    marginLeft: 76,
    marginRight: 20,
  },

  /* ================= APP INFO ================= */
  appInfoSection: {
    alignItems: 'center',
    paddingVertical: 16,
  },

  appInfoTitle: {
    color: '#5F3BFF',
    fontSize: 18,
    fontWeight: '800',
    marginBottom: 8,
  },

  versionText: {
    textAlign: 'center',
    color: '#A0A0B0',
    fontSize: 13,
    fontWeight: '500',
    marginBottom: 4,
  },

  copyrightText: {
    textAlign: 'center',
    color: '#C0C0D0',
    fontSize: 11,
    fontWeight: '400',
  },

  /* ================= BOTTOM NAV ================= */
  bottomNav: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    flexDirection: 'row',
    backgroundColor: '#FFFFFF',
    paddingVertical: 12,
    paddingBottom: 24,
    paddingHorizontal: 32,
    justifyContent: 'space-around',
    borderTopWidth: 1,
    borderTopColor: '#F0F0F5',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: -4 },
    shadowOpacity: 0.08,
    shadowRadius: 12,
    elevation: 10,
  },

  navItem: {
    padding: 8,
    alignItems: 'center',
    justifyContent: 'center',
    position: 'relative',
  },

  activeIndicator: {
    position: 'absolute',
    bottom: 0,
    width: 32,
    height: 3,
    backgroundColor: '#5F3BFF',
    borderRadius: 2,
  },
});