import React, { useState, useEffect } from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Image,
  StatusBar,
  Alert,
} from 'react-native'

import { useNavigation, useRoute } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function ManageChildrenScreen() {
  const navigation = useNavigation()
  const route = useRoute()

  // Sample children data - would come from API/database
  const [children, setChildren] = useState([
    {
      id: '1',
      name: 'Emma',
      age: '6-8',
      avatar: require('../../../../../assets/images/child1.png'),
      loginCode: 'EMM2024',
      balance: 120,
      status: 'active'
    },
    {
      id: '2',
      name: 'Jake',
      age: '9-11',
      avatar: require('../../../../../assets/images/child2.png'),
      loginCode: 'JAK2024',
      balance: 80,
      status: 'active'
    },
  ])

  // Handle new child from profiling
  useEffect(() => {
    if (route.params?.newChild) {
      const newChildData = route.params.newChild
      
      const newChild = {
        id: Date.now().toString(),
        name: newChildData.childName,
        age: newChildData.ageGroup,
        avatar: require('../../../../../assets/images/default-avatar.png'),
        loginCode: newChildData.loginCode,
        balance: 0,
        status: 'active',
        profileData: newChildData // Store all profiling data
      }

      setChildren(prevChildren => [...prevChildren, newChild])
      
      // Clear the params
      navigation.setParams({ newChild: undefined })
    }
  }, [route.params?.newChild])

  const handleDeleteChild = (child) => {
    Alert.alert(
      'Remove Child',
      `Are you sure you want to remove ${child.name}?\n\nThis will delete all their data including expenses, goals, and achievements.`,
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Remove',
          style: 'destructive',
          onPress: () => {
            setChildren(children.filter(c => c.id !== child.id))
            Alert.alert('Success', `${child.name} has been removed`)
          }
        }
      ]
    )
  }

  const handleViewDetails = (child) => {
    console.log('View details for: - Managechildrenscreen.js:85', child.name)
    // navigation.navigate('ChildDetails', { childId: child.id })
  }

  const handleCopyCode = (code, name) => {
    Alert.alert(
      'Login Code Copied',
      `${name}'s login code: ${code}\n\nThe code has been copied to clipboard.`,
      [{ text: 'OK' }]
    )
  }

  const handleAddNewChild = () => {
    navigation.navigate('AddNewChild')
  }

  const renderChildCard = (child) => {
    return (
      <View key={child.id} style={styles.childCard}>
        {/* Child Info Section */}
        <View style={styles.childHeader}>
          <View style={styles.childInfoLeft}>
            <Image source={child.avatar} style={styles.childAvatar} />
            <View style={styles.childDetails}>
              <Text style={styles.childName}>{child.name}</Text>
              <Text style={styles.childAge}>{child.age} years</Text>
              <View style={styles.balanceContainer}>
                <Ionicons name="wallet" size={14} color="#5F3BFF" />
                <Text style={styles.balanceText}>${child.balance}</Text>
              </View>
            </View>
          </View>

          <View style={styles.statusBadge}>
            <View style={styles.statusDot} />
            <Text style={styles.statusText}>Active</Text>
          </View>
        </View>

        {/* Login Code Section */}
        <View style={styles.codeSection}>
          <View style={styles.codeLeft}>
            <Ionicons name="key" size={18} color="#5F3BFF" />
            <Text style={styles.codeLabel}>Login Code:</Text>
            <Text style={styles.codeValue}>{child.loginCode}</Text>
          </View>
          
          <TouchableOpacity 
            style={styles.copyButton}
            onPress={() => handleCopyCode(child.loginCode, child.name)}
          >
            <Ionicons name="copy-outline" size={18} color="#5F3BFF" />
          </TouchableOpacity>
        </View>

        {/* Action Buttons */}
        <View style={styles.actionButtons}>
          <TouchableOpacity 
            style={styles.actionBtn}
            onPress={() => handleViewDetails(child)}
          >
            <Ionicons name="eye" size={20} color="#5F3BFF" />
            <Text style={styles.actionBtnText}>View Details</Text>
          </TouchableOpacity>

          <TouchableOpacity 
            style={[styles.actionBtn, styles.deleteBtn]}
            onPress={() => handleDeleteChild(child)}
          >
            <Ionicons name="trash" size={20} color="#FF6B9D" />
            <Text style={[styles.actionBtnText, styles.deleteBtnText]}>Remove</Text>
          </TouchableOpacity>
        </View>
      </View>
    )
  }

  return (
    <View style={styles.container}>
      <StatusBar barStyle="light-content" />

      {/* Header */}
      <LinearGradient
        colors={['#1F1147', '#2B1055', '#4B0082', '#5F3BFF']}
        start={{ x: 0, y: 0 }}
        end={{ x: 1, y: 1 }}
        style={styles.header}
      >
        <View style={styles.headerContent}>
          <TouchableOpacity 
            style={styles.backButton}
            onPress={() => navigation.goBack()}
          >
            <Ionicons name="arrow-back" size={24} color="#fff" />
          </TouchableOpacity>

          <View style={styles.headerTitle}>
            <Text style={styles.headerTitleText}>Manage Children</Text>
            <Text style={styles.headerSubtitle}>{children.length} {children.length === 1 ? 'Child' : 'Children'}</Text>
          </View>

          <View style={{ width: 40 }} />
        </View>
      </LinearGradient>

      {/* Children List */}
      <ScrollView 
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* Info Card */}
        <View style={styles.infoCard}>
          <Ionicons name="information-circle" size={24} color="#5F3BFF" />
          <Text style={styles.infoText}>
            Each child needs a unique login code to access their account. Keep these codes safe!
          </Text>
        </View>

        {/* Children Cards */}
        {children.map(child => renderChildCard(child))}

        {/* Add Child Button */}
        <TouchableOpacity 
          style={styles.addChildButton}
          onPress={handleAddNewChild}
        >
          <LinearGradient
            colors={['#5F3BFF', '#3B1DFF', '#7C4DFF']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
            style={styles.addChildGradient}
          >
            <Ionicons name="add-circle" size={24} color="#FFFFFF" />
            <Text style={styles.addChildText}>Add New Child</Text>
          </LinearGradient>
        </TouchableOpacity>
      </ScrollView>
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
    borderBottomLeftRadius: 30,
    borderBottomRightRadius: 30,
    shadowColor: '#3B1DFF',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.7,
    shadowRadius: 25,
    elevation: 10,
  },

  headerContent: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 24,
  },

  backButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: 'rgba(255, 255, 255, 0.2)',
    justifyContent: 'center',
    alignItems: 'center',
  },

  headerTitle: {
    alignItems: 'center',
    flex: 1,
  },

  headerTitleText: {
    color: '#fff',
    fontSize: 22,
    fontWeight: '800',
    letterSpacing: 0.5,
  },

  headerSubtitle: {
    color: 'rgba(255, 255, 255, 0.8)',
    fontSize: 14,
    fontWeight: '500',
    marginTop: 2,
  },

  /* ================= SCROLL VIEW ================= */
  scrollView: {
    flex: 1,
  },

  scrollContent: {
    padding: 24,
    paddingBottom: 120,
  },

  /* ================= INFO CARD ================= */
  infoCard: {
    backgroundColor: '#E8E1FF',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 24,
    borderWidth: 1,
    borderColor: '#D0C4FF',
  },

  infoText: {
    flex: 1,
    marginLeft: 12,
    color: '#5F3BFF',
    fontSize: 14,
    fontWeight: '600',
    lineHeight: 20,
  },

  /* ================= CHILD CARD ================= */
  childCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 20,
    padding: 20,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.08,
    shadowRadius: 12,
    elevation: 4,
    borderWidth: 1,
    borderColor: '#F0F0F5',
  },

  childHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 16,
  },

  childInfoLeft: {
    flexDirection: 'row',
    flex: 1,
  },

  childAvatar: {
    width: 60,
    height: 60,
    borderRadius: 30,
    borderWidth: 2,
    borderColor: '#5F3BFF',
    marginRight: 12,
  },

  childDetails: {
    flex: 1,
  },

  childName: {
    color: '#231c63',
    fontSize: 18,
    fontWeight: '800',
    marginBottom: 4,
  },

  childAge: {
    color: '#8A8FB0',
    fontSize: 14,
    fontWeight: '500',
    marginBottom: 6,
  },

  balanceContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#F0ECFF',
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 12,
    alignSelf: 'flex-start',
  },

  balanceText: {
    color: '#5F3BFF',
    fontSize: 14,
    fontWeight: '700',
    marginLeft: 4,
  },

  statusBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#E8F9F6',
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 12,
  },

  statusDot: {
    width: 6,
    height: 6,
    borderRadius: 3,
    backgroundColor: '#00D4AA',
    marginRight: 6,
  },

  statusText: {
    color: '#00D4AA',
    fontSize: 12,
    fontWeight: '700',
  },

  /* ================= CODE SECTION ================= */
  codeSection: {
    backgroundColor: '#F8F6FF',
    borderRadius: 12,
    padding: 14,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
    borderWidth: 1,
    borderColor: '#E6E0FF',
  },

  codeLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },

  codeLabel: {
    color: '#8A8FB0',
    fontSize: 14,
    fontWeight: '600',
    marginLeft: 8,
  },

  codeValue: {
    color: '#5F3BFF',
    fontSize: 16,
    fontWeight: '800',
    marginLeft: 8,
    letterSpacing: 1,
  },

  copyButton: {
    padding: 8,
    backgroundColor: '#FFFFFF',
    borderRadius: 8,
  },

  /* ================= ACTION BUTTONS ================= */
  actionButtons: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    gap: 12,
  },

  actionBtn: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#F0ECFF',
    paddingVertical: 12,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: '#E6E0FF',
  },

  actionBtnText: {
    color: '#5F3BFF',
    fontSize: 14,
    fontWeight: '700',
    marginLeft: 6,
  },

  deleteBtn: {
    backgroundColor: '#FFF0F5',
    borderColor: '#FFE0EB',
  },

  deleteBtnText: {
    color: '#FF6B9D',
  },

  /* ================= ADD CHILD BUTTON ================= */
  addChildButton: {
    marginTop: 24,
    marginBottom: 40,
    borderRadius: 16,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 12,
    elevation: 6,
  },

  addChildGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 18,
    paddingHorizontal: 24,
  },

  addChildText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '800',
    marginLeft: 8,
  },
});