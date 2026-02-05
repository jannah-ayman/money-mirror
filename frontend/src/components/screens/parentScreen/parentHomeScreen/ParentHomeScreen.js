import React, { useState, useRef } from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  FlatList,
  Animated,
  ScrollView,
  Image,
  StatusBar
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient' 

const childrenData = [
  {
    id: '1',
    name: 'Emma',
    age: '4 months',
    avatar: require('../../../../../assets/images/child1.png'),
    balance: 120,
    spent: 35,
  },
  {
    id: '2',
    name: 'Jake',
    age: '7 years',
    avatar: require('../../../../../assets/images/child2.png'),
    balance: 80,
    spent: 50,
  },
]

export default function ParentHome() {
  const navigation = useNavigation()
  const [activeChild, setActiveChild] = useState(childrenData[0])
  const fadeAnim = useRef(new Animated.Value(1)).current
  const scaleAnim = useRef(new Animated.Value(1)).current

  const animateSwitch = (child) => {
    Animated.sequence([
      Animated.parallel([
        Animated.timing(fadeAnim, {
          toValue: 0,
          duration: 200,
          useNativeDriver: true,
        }),
        Animated.timing(scaleAnim, {
          toValue: 0.95,
          duration: 200,
          useNativeDriver: true,
        }),
      ]),
      Animated.parallel([
        Animated.timing(fadeAnim, {
          toValue: 1,
          duration: 250,
          useNativeDriver: true,
        }),
        Animated.timing(scaleAnim, {
          toValue: 1,
          duration: 250,
          useNativeDriver: true,
        }),
      ]),
    ]).start()

    setTimeout(() => setActiveChild(child), 200)
  }

  const renderProfile = ({ item }) => {
    const active = item.id === activeChild.id

    return (
      <TouchableOpacity onPress={() => animateSwitch(item)}>
        <View style={[
          styles.profileCard,
          active && styles.activeProfile
        ]}>
          <Image source={item.avatar} style={[
            styles.avatar,
            active && styles.activeAvatar
          ]} />
          <Text style={styles.profileName}>{item.name}</Text>
        </View>
      </TouchableOpacity>
    )
  }

  const renderManageChildrenButton = () => {
    return (
      <TouchableOpacity 
        style={styles.addChildButton}
        onPress={() => {
          console.log('Navigate to Manage Children - ParentHomeScreen.js:98')
          navigation.navigate('ManageChildren')
        }}
      >
        <LinearGradient
          colors={['#5F3BFF', '#3B1DFF', '#7C4DFF']}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.addChildGradient}
        >
          <Ionicons name="people" size={32} color="#FFFFFF" />
        </LinearGradient>
        <Text style={styles.addChildText}>Manage Children</Text>
      </TouchableOpacity>
    )
  }

  return (
    <View style={styles.container}>
      <StatusBar barStyle="light-content" />

      {/* ================= HEADER ================= */}
      <LinearGradient
        colors={['#1F1147', '#2B1055', '#4B0082']}
        start={{ x: 0, y: 0 }}
        end={{ x: 1, y: 1 }}
        style={styles.header}
      >
        <View style={styles.greetingContainer}>
          <Text style={styles.greetingText}>Hi Jenifer!</Text>
          <Text style={styles.subGreetingText}>Good Morning</Text>
        </View>

        <TouchableOpacity 
          style={styles.notification}
          onPress={() => {
            console.log('Navigate to Notifications - ParentHomeScreen.js:134')
            navigation.navigate('NotificationsScreen')
          }}
        >
          <Ionicons name="notifications" size={24} color="#fff" />
          <View style={styles.badge}>
            <Text style={styles.badgeText}>3</Text>
          </View>
        </TouchableOpacity>
      </LinearGradient>

      {/* Profiles with Manage Children Button */}
      <FlatList
        data={childrenData}
        horizontal
        showsHorizontalScrollIndicator={false}
        keyExtractor={item => item.id}
        renderItem={renderProfile}
        ListFooterComponent={renderManageChildrenButton}
        style={styles.profileList}
        contentContainerStyle={styles.profileListContent}
      />

      {/* Main Animated Content */}
      <Animated.View
        style={{
          opacity: fadeAnim,
          transform: [{ scale: scaleAnim }],
          flex: 1
        }}
      >
        <ScrollView showsVerticalScrollIndicator={false}>

          {/* Info Cards */}
          <View style={styles.infoRow}>
            <View style={styles.infoCard}>
              <Text style={styles.infoTitle}>Balance</Text>
              <Text style={styles.infoValue}>${activeChild.balance}</Text>
            </View>

            <View style={styles.infoCard}>
              <Text style={styles.infoTitle}>Spent This Week</Text>
              <Text style={styles.infoValue}>${activeChild.spent}</Text>
            </View>
          </View>

          {/* Actions */}
          <View style={styles.actionsRow}>
            <TouchableOpacity 
              style={styles.actionBtn}
              onPress={() => {
                console.log('Navigate to Allowance Management - ParentHomeScreen.js:185')
                // navigation.navigate('AllowanceManagement', { childId: activeChild.id })
              }}
            >
              <Ionicons name="wallet" size={24} color="#00E0C6" />
              <Text style={styles.actionText}>Allowance</Text>
            </TouchableOpacity>

            <TouchableOpacity 
              style={styles.actionBtn}
              onPress={() => {
                console.log('Navigate to Goals & Challenges - ParentHomeScreen.js:196')
                // navigation.navigate('GoalsChallenges', { childId: activeChild.id })
              }}
            >
              <Ionicons name="trophy" size={24} color="#FFD700" />
              <Text style={styles.actionText}>Goals</Text>
            </TouchableOpacity>
          </View>

          {/* Focus Section */}
          <View style={styles.focusCard}>
            <Text style={styles.focusTitle}>Think Time</Text>
            <Text style={styles.focusDesc}>
              Always wait 24 hours before buying toys.
              Ask yourself: Do I really need it?
            </Text>
            <View style={styles.metric}>
              <Text style={styles.metricText}>
                🎯 Save $20 this week
              </Text>
            </View>
          </View>

          {/* Activity */}
          <View style={styles.activityHeader}>
            <Text style={styles.sectionTitle}>Recent Activity</Text>
            <TouchableOpacity 
              onPress={() => {
                console.log('View All Activity - ParentHomeScreen.js:224')
                // navigation.navigate('ExpenseHistory', { childId: activeChild.id })
              }}
            >
              <Text style={styles.viewAllText}>View All</Text>
            </TouchableOpacity>
          </View>

          <View style={styles.activityCard}>
            <Text style={styles.activityText}>🧸 Toy Store • Today • 😊 • -$15</Text>
          </View>

          <View style={styles.activityCard}>
            <Text style={styles.activityText}>🍭 Candy Shop • Yesterday • 😄 • -$5</Text>
          </View>

          <View style={styles.activityCard}>
            <Text style={styles.activityText}>📚 Book Store • 2d ago • 🙂 • -$10</Text>
          </View>

        </ScrollView>
      </Animated.View>

      {/* Bottom Navigation */}
      <View style={styles.bottomNav}>
        <TouchableOpacity style={styles.navItem}>
          <Ionicons name="home" size={24} color="#5F3BFF" />
          <View style={styles.activeIndicator} />
        </TouchableOpacity>

        <TouchableOpacity 
          style={styles.navItem}
          onPress={() => {
            console.log('Navigate to Reports - ParentHomeScreen.js:257')
            // navigation.navigate('Reports')
          }}
        >
          <Ionicons name="stats-chart" size={24} color="#C0C0D0" />
        </TouchableOpacity>

        <TouchableOpacity 
          style={styles.navItem}
          onPress={() => {
            console.log('Navigate to Notifications - ParentHomeScreen.js:267')
            // navigation.navigate('Notifications')
          }}
        >
          <Ionicons name="notifications" size={24} color="#C0C0D0" />
        </TouchableOpacity>

        <TouchableOpacity 
          style={styles.navItem}
          onPress={() => {
            console.log('Navigate to Profile - ParentHomeScreen.js:277')
            navigation.navigate('ParentProfile')
          }}
        >
          <Ionicons name="person" size={24} color="#C0C0D0" />
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
    paddingBottom: 24,
    paddingHorizontal: 24,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    borderBottomLeftRadius: 30,
    borderBottomRightRadius: 30,
    shadowColor: '#3B1DFF',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.7,
    shadowRadius: 25,
    elevation: 10,
  },

  greetingContainer: {
    flex: 1,
    marginLeft: 16,
  },

  greetingText: {
    color: '#fff',
    fontSize: 20,
    fontWeight: '800',
    letterSpacing: 0.5,
  },

  subGreetingText: {
    color: 'rgba(255, 255, 255, 0.7)',
    fontSize: 13,
    fontWeight: '500',
    marginTop: 2,
  },

  notification: {
    position: 'relative',
  },

  badge: {
    position: 'absolute',
    right: -6,
    top: -4,
    backgroundColor: '#FF6B9D',
    width: 20,
    height: 20,
    borderRadius: 10,
    alignItems: 'center',
    justifyContent: 'center',
    borderWidth: 2,
    borderColor: '#fff',
    shadowColor: '#FF6B9D',
    shadowOffset: { width: 0, height: 0 },
    shadowOpacity: 0.8,
    shadowRadius: 8,
    elevation: 6,
  },

  badgeText: {
    color: '#fff',
    fontSize: 10,
    fontWeight: '800',
  },

  /* ================= PROFILES ================= */
  profileList: {
    flexGrow: 0,
    paddingVertical: 16,
  },

  profileListContent: {
    paddingHorizontal: 20,
  },

  profileCard: {
    alignItems: 'center',
    marginRight: 20,
    paddingBottom: 10,
    paddingHorizontal: 12,
    paddingTop: 8,
  },

  activeProfile: {
    backgroundColor: 'rgba(95, 59, 255, 0.18)',
    borderRadius: 24,
    borderWidth: 2,
    borderColor: '#5F3BFF',
    paddingVertical: 12,
    paddingHorizontal: 16,
    shadowColor: '#3B1DFF',
    shadowOffset: { width: 0, height: 0 },
    shadowOpacity: 0.8,
    shadowRadius: 16,
    elevation: 10,
    transform: [{ scale: 1.05 }],
  },

  avatar: {
    width: 52,
    height: 52,
    borderRadius: 26,
    marginBottom: 8,
    borderWidth: 2,
    borderColor: 'transparent',
  },

  activeAvatar: {
    width: 64,
    height: 64,
    borderRadius: 32,
    borderWidth: 3,
    borderColor: '#5F3BFF',
  },

  profileName: {
    color: '#231c63',
    fontWeight: '700',
    fontSize: 15,
    marginTop: 4,
  },

  /* ================= MANAGE CHILDREN BUTTON ================= */
  addChildButton: {
    alignItems: 'center',
    marginRight: 20,
    paddingBottom: 10,
    paddingHorizontal: 12,
    paddingTop: 8,
  },

  addChildGradient: {
    width: 60,
    height: 60,
    borderRadius: 30,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 8,
    borderWidth: 2,
    borderColor: '#FFFFFF',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.4,
    shadowRadius: 12,
    elevation: 6,
  },

  addChildText: {
    color: '#5F3BFF',
    fontWeight: '700',
    fontSize: 13,
    marginTop: 4,
  },

  /* ================= INFO CARDS ================= */
  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingHorizontal: 24,
    marginTop: 8,
    marginBottom: 4,
  },

  infoCard: {
    backgroundColor: '#FFFFFF',
    width: '48%',
    padding: 18,
    borderRadius: 16,
    borderWidth: 1,
    borderColor: '#E6E8F0',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },

  infoTitle: {
    color: '#8A8FB0',
    fontSize: 13,
    fontWeight: '600',
    letterSpacing: 0.3,
  },

  infoValue: {
    color: '#231c63',
    fontSize: 26,
    fontWeight: '800',
    marginTop: 8,
  },

  /* ================= ACTIONS ================= */
  actionsRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingHorizontal: 24,
    marginVertical: 24,
  },

  actionBtn: {
    backgroundColor: '#FFFFFF',
    width: '48%',
    paddingVertical: 16,
    borderRadius: 16,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#E6E8F0',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },

  actionText: {
    color: '#2A236F',
    marginTop: 8,
    fontWeight: '700',
    fontSize: 15,
  },

  /* ================= FOCUS SECTION ================= */
  focusCard: {
    backgroundColor: '#FFFFFF',
    marginHorizontal: 24,
    padding: 20,
    borderRadius: 18,
    borderWidth: 1,
    borderColor: '#E6E8F0',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 3 },
    shadowOpacity: 0.08,
    shadowRadius: 6,
    elevation: 3,
  },

  focusTitle: {
    color: '#231c63',
    fontSize: 18,
    fontWeight: '800',
    marginBottom: 4,
  },

  focusDesc: {
    color: '#5A5A72',
    marginVertical: 12,
    lineHeight: 22,
    fontSize: 14,
    fontWeight: '500',
  },

  metric: {
    backgroundColor: '#1E1B3A',
    padding: 14,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: '#5F3BFF',
    marginTop: 8,
  },

  metricText: {
    color: '#A78BFA',
    fontWeight: '700',
    fontSize: 15,
  },

  /* ================= ACTIVITY ================= */
  activityHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginHorizontal: 24,
    marginTop: 24,
    marginBottom: 16,
  },

  sectionTitle: {
    color: '#231c63',
    fontSize: 19,
    fontWeight: '800',
  },

  viewAllText: {
    color: '#5F3BFF',
    fontSize: 14,
    fontWeight: '700',
  },

  activityCard: {
    backgroundColor: '#FFFFFF',
    marginHorizontal: 24,
    marginBottom: 12,
    padding: 16,
    borderRadius: 16,
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#E6E8F0',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.04,
    shadowRadius: 4,
    elevation: 1,
  },

  activityText: {
    color: '#231c63',
    fontSize: 15,
    fontWeight: '600',
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