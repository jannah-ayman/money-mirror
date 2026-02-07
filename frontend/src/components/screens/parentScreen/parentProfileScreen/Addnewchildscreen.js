import React, { useState } from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  StatusBar,
  TextInput,
  Alert,
  Modal,
  FlatList
} from 'react-native'

import { useNavigation } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function AddNewChildScreen() {
  const navigation = useNavigation()
  const [childName, setChildName] = useState('')
  const [selectedDay, setSelectedDay] = useState(1)
  const [selectedMonth, setSelectedMonth] = useState(1)
  const [selectedYear, setSelectedYear] = useState(2015)
  const [showDateModal, setShowDateModal] = useState(false)
  const [currentPicker, setCurrentPicker] = useState('day') // 'day', 'month', 'year'

  const days = Array.from({ length: 31 }, (_, i) => i + 1)
  const months = [
    { value: 1, label: 'January' },
    { value: 2, label: 'February' },
    { value: 3, label: 'March' },
    { value: 4, label: 'April' },
    { value: 5, label: 'May' },
    { value: 6, label: 'June' },
    { value: 7, label: 'July' },
    { value: 8, label: 'August' },
    { value: 9, label: 'September' },
    { value: 10, label: 'October' },
    { value: 11, label: 'November' },
    { value: 12, label: 'December' }
  ]
  const currentYear = new Date().getFullYear()
  const years = Array.from({ length: 13 }, (_, i) => currentYear - 5 - i) // 2020 to 2007

  const formatDate = () => {
    const monthName = months.find(m => m.value === selectedMonth)?.label || ''
    return `${selectedDay} ${monthName} ${selectedYear}`
  }

  const calculateAge = () => {
    const today = new Date()
    const birthDate = new Date(selectedYear, selectedMonth - 1, selectedDay)
    let age = today.getFullYear() - birthDate.getFullYear()
    const monthDiff = today.getMonth() - birthDate.getMonth()
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--
    }
    
    return age
  }

  const handleContinue = () => {
    if (!childName.trim()) {
      Alert.alert('Required', 'Please enter child name')
      return
    }
    
    const age = calculateAge()
    
    if (age < 5 || age > 17) {
      Alert.alert('Age Restriction', 'Child age must be between 5 and 17 years old')
      return
    }

    const birthDate = new Date(selectedYear, selectedMonth - 1, selectedDay)

    // Navigate to Initial Profiling Survey
    navigation.navigate('InitialProfiling', {
      childName: childName.trim(),
      birthDate: birthDate.toISOString()
    })
  }

  const openPicker = (type) => {
    setCurrentPicker(type)
    setShowDateModal(true)
  }

  const renderPickerItem = ({ item }) => {
    let isSelected = false
    let label = ''

    if (currentPicker === 'day') {
      isSelected = item === selectedDay
      label = item.toString()
    } else if (currentPicker === 'month') {
      isSelected = item.value === selectedMonth
      label = item.label
    } else {
      isSelected = item === selectedYear
      label = item.toString()
    }

    return (
      <TouchableOpacity
        style={[styles.pickerItem, isSelected && styles.pickerItemSelected]}
        onPress={() => {
          if (currentPicker === 'day') setSelectedDay(item)
          else if (currentPicker === 'month') setSelectedMonth(item.value)
          else setSelectedYear(item)
          setShowDateModal(false)
        }}
      >
        <Text style={[styles.pickerItemText, isSelected && styles.pickerItemTextSelected]}>
          {label}
        </Text>
        {isSelected && <Ionicons name="checkmark-circle" size={24} color="#5F3BFF" />}
      </TouchableOpacity>
    )
  }

  const age = calculateAge()

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
            <Text style={styles.headerTitleText}>Add New Child</Text>
            <Text style={styles.headerSubtitle}>Step 1 of 2</Text>
          </View>

          <View style={{ width: 40 }} />
        </View>
      </LinearGradient>

      {/* Content */}
      <ScrollView 
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* Progress Bar */}
        <View style={styles.progressContainer}>
          <View style={styles.progressBar}>
            <View style={[styles.progressFill, { width: '50%' }]} />
          </View>
          <Text style={styles.progressText}>Basic Information</Text>
        </View>

        {/* Info Card */}
        <View style={styles.infoCard}>
          <Ionicons name="information-circle" size={24} color="#5F3BFF" />
          <Text style={styles.infoText}>
            We'll ask a few questions to understand your child's financial personality better
          </Text>
        </View>

        {/* Child Name Input */}
        <View style={styles.inputSection}>
          <Text style={styles.sectionLabel}>Child's Name</Text>
          <View style={styles.inputContainer}>
            <Ionicons name="person-outline" size={22} color="#8A8FB0" />
            <TextInput
              style={styles.input}
              placeholder="Enter child's name"
              placeholderTextColor="#C0C0D0"
              value={childName}
              onChangeText={setChildName}
              autoCapitalize="words"
            />
          </View>
        </View>

        {/* Birth Date Selection */}
        <View style={styles.inputSection}>
          <Text style={styles.sectionLabel}>Birth Date</Text>
          <Text style={styles.sectionSubtitle}>Select your child's date of birth</Text>
          
          {/* Date Display */}
          <View style={styles.dateDisplayCard}>
            <View style={styles.dateDisplayHeader}>
              <Ionicons name="calendar" size={24} color="#5F3BFF" />
              <Text style={styles.dateDisplayText}>{formatDate()}</Text>
            </View>
            <Text style={styles.ageDisplayText}>{age} years old</Text>
          </View>

          {/* Date Pickers Row */}
          <View style={styles.datePickersRow}>
            <TouchableOpacity
              style={styles.datePickerButton}
              onPress={() => openPicker('day')}
            >
              <Text style={styles.datePickerLabel}>Day</Text>
              <Text style={styles.datePickerValue}>{selectedDay}</Text>
              <Ionicons name="chevron-down" size={18} color="#8A8FB0" />
            </TouchableOpacity>

            <TouchableOpacity
              style={styles.datePickerButton}
              onPress={() => openPicker('month')}
            >
              <Text style={styles.datePickerLabel}>Month</Text>
              <Text style={styles.datePickerValue}>
                {months.find(m => m.value === selectedMonth)?.label.substring(0, 3)}
              </Text>
              <Ionicons name="chevron-down" size={18} color="#8A8FB0" />
            </TouchableOpacity>

            <TouchableOpacity
              style={styles.datePickerButton}
              onPress={() => openPicker('year')}
            >
              <Text style={styles.datePickerLabel}>Year</Text>
              <Text style={styles.datePickerValue}>{selectedYear}</Text>
              <Ionicons name="chevron-down" size={18} color="#8A8FB0" />
            </TouchableOpacity>
          </View>
        </View>

        {/* Age Info Card */}
        {age >= 5 && age <= 17 && (
          <View style={styles.ageInfoCard}>
            <View style={styles.ageIconContainer}>
              <Ionicons name="checkmark-circle" size={28} color="#00D4AA" />
            </View>
            <View style={styles.ageInfoText}>
              <Text style={styles.ageInfoTitle}>Perfect! ✨</Text>
              <Text style={styles.ageInfoSubtitle}>
                {childName.trim() || 'Your child'} is {age} years old and eligible for Money Mirror
              </Text>
            </View>
          </View>
        )}

        {(age < 5 || age > 17) && age > 0 && (
          <View style={styles.ageWarningCard}>
            <Ionicons name="alert-circle" size={24} color="#FF6B9D" />
            <Text style={styles.warningText}>
              Money Mirror is designed for children aged 5-17 years
            </Text>
          </View>
        )}

        {/* Character Guide */}
        <View style={styles.characterGuide}>
          <View style={styles.characterBubble}>
            <Text style={styles.characterText}>
              Great! Next, I'll help you understand your child's spending personality! 🌟
            </Text>
          </View>
          <Text style={styles.characterEmoji}>🦊</Text>
        </View>
      </ScrollView>

      {/* Continue Button */}
      <View style={styles.buttonContainer}>
        <TouchableOpacity 
          style={[
            styles.continueButton,
            (!childName.trim() || age < 5 || age > 17) && styles.continueButtonDisabled
          ]}
          onPress={handleContinue}
          disabled={!childName.trim() || age < 5 || age > 17}
        >
          <LinearGradient
            colors={
              childName.trim() && age >= 5 && age <= 17
                ? ['#5F3BFF', '#3B1DFF', '#7C4DFF'] 
                : ['#C0C0D0', '#A0A0B0']
            }
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
            style={styles.continueGradient}
          >
            <Text style={styles.continueText}>Continue to Survey</Text>
            <Ionicons name="arrow-forward" size={20} color="#FFFFFF" />
          </LinearGradient>
        </TouchableOpacity>
      </View>

      {/* Date Picker Modal */}
      <Modal
        visible={showDateModal}
        animationType="slide"
        transparent={true}
        onRequestClose={() => setShowDateModal(false)}
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>
                Select {currentPicker === 'day' ? 'Day' : currentPicker === 'month' ? 'Month' : 'Year'}
              </Text>
              <TouchableOpacity onPress={() => setShowDateModal(false)}>
                <Ionicons name="close" size={24} color="#231c63" />
              </TouchableOpacity>
            </View>

            <FlatList
              data={currentPicker === 'day' ? days : currentPicker === 'month' ? months : years}
              renderItem={renderPickerItem}
              keyExtractor={(item) => 
                currentPicker === 'month' ? item.value.toString() : item.toString()
              }
              showsVerticalScrollIndicator={false}
              contentContainerStyle={styles.pickerList}
            />
          </View>
        </View>
      </Modal>
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
    paddingBottom: 160,
  },

  /* ================= PROGRESS BAR ================= */
  progressContainer: {
    marginBottom: 24,
  },

  progressBar: {
    height: 8,
    backgroundColor: '#E8E1FF',
    borderRadius: 4,
    overflow: 'hidden',
    marginBottom: 8,
  },

  progressFill: {
    height: '100%',
    backgroundColor: '#5F3BFF',
    borderRadius: 4,
  },

  progressText: {
    color: '#5F3BFF',
    fontSize: 13,
    fontWeight: '700',
    textAlign: 'center',
  },

  /* ================= INFO CARD ================= */
  infoCard: {
    backgroundColor: '#E8E1FF',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 28,
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

  /* ================= INPUT SECTION ================= */
  inputSection: {
    marginBottom: 28,
  },

  sectionLabel: {
    color: '#231c63',
    fontSize: 16,
    fontWeight: '800',
    marginBottom: 4,
  },

  sectionSubtitle: {
    color: '#8A8FB0',
    fontSize: 13,
    fontWeight: '500',
    marginBottom: 12,
  },

  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    paddingHorizontal: 18,
    borderWidth: 2,
    borderColor: '#E6E0FF',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 8,
    elevation: 2,
  },

  input: {
    flex: 1,
    paddingVertical: 16,
    paddingLeft: 12,
    color: '#231c63',
    fontSize: 16,
    fontWeight: '600',
  },

  /* ================= DATE PICKER ================= */
  dateDisplayCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    padding: 20,
    borderWidth: 2,
    borderColor: '#5F3BFF',
    marginBottom: 16,
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.15,
    shadowRadius: 12,
    elevation: 4,
  },

  dateDisplayHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 8,
  },

  dateDisplayText: {
    color: '#231c63',
    fontSize: 20,
    fontWeight: '800',
    marginLeft: 12,
  },

  ageDisplayText: {
    color: '#5F3BFF',
    fontSize: 16,
    fontWeight: '700',
    marginLeft: 36,
  },

  datePickersRow: {
    flexDirection: 'row',
    gap: 10,
  },

  datePickerButton: {
    flex: 1,
    backgroundColor: '#FFFFFF',
    borderRadius: 12,
    padding: 14,
    borderWidth: 2,
    borderColor: '#E6E0FF',
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 6,
    elevation: 2,
  },

  datePickerLabel: {
    color: '#8A8FB0',
    fontSize: 12,
    fontWeight: '600',
    marginBottom: 6,
  },

  datePickerValue: {
    color: '#231c63',
    fontSize: 18,
    fontWeight: '800',
    marginBottom: 4,
  },

  /* ================= AGE INFO CARDS ================= */
  ageInfoCard: {
    backgroundColor: '#E8F9F6',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 20,
    borderWidth: 1,
    borderColor: '#B8F0E6',
  },

  ageIconContainer: {
    marginRight: 12,
  },

  ageInfoText: {
    flex: 1,
  },

  ageInfoTitle: {
    color: '#00D4AA',
    fontSize: 16,
    fontWeight: '800',
    marginBottom: 4,
  },

  ageInfoSubtitle: {
    color: '#00A688',
    fontSize: 14,
    fontWeight: '600',
    lineHeight: 20,
  },

  ageWarningCard: {
    backgroundColor: '#FFF0F5',
    borderRadius: 16,
    padding: 16,
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 20,
    borderWidth: 1,
    borderColor: '#FFD0E0',
  },

  warningText: {
    flex: 1,
    marginLeft: 12,
    color: '#FF6B9D',
    fontSize: 14,
    fontWeight: '600',
    lineHeight: 20,
  },

  /* ================= CHARACTER GUIDE ================= */
  characterGuide: {
    marginTop: 8,
    alignItems: 'center',
  },

  characterBubble: {
    backgroundColor: '#FFFFFF',
    borderRadius: 20,
    padding: 16,
    marginBottom: 12,
    borderWidth: 2,
    borderColor: '#FFD93D',
    maxWidth: '85%',
    shadowColor: '#FFD93D',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.2,
    shadowRadius: 8,
    elevation: 4,
  },

  characterText: {
    color: '#231c63',
    fontSize: 14,
    fontWeight: '600',
    textAlign: 'center',
    lineHeight: 20,
  },

  characterEmoji: {
    fontSize: 48,
  },

  /* ================= BUTTON ================= */
  buttonContainer: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    padding: 24,
    paddingTop: 16,
    paddingBottom: 80,
    backgroundColor: '#FFFFFF',
    borderTopWidth: 1,
    borderTopColor: '#E6E0FF',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: -4 },
    shadowOpacity: 0.1,
    shadowRadius: 12,
    elevation: 8,
  },

  continueButton: {
    borderRadius: 16,
    overflow: 'hidden',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 12,
    elevation: 6,
  },

  continueButtonDisabled: {
    shadowOpacity: 0.1,
  },

  continueGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 18,
    paddingHorizontal: 24,
    gap: 8,
  },

  continueText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '800',
  },

  /* ================= MODAL PICKER ================= */
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    justifyContent: 'flex-end',
  },

  modalContent: {
    backgroundColor: '#FFFFFF',
    borderTopLeftRadius: 30,
    borderTopRightRadius: 30,
    paddingBottom: 40,
    maxHeight: '60%',
  },

  modalHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingHorizontal: 24,
    paddingTop: 24,
    paddingBottom: 16,
    borderBottomWidth: 1,
    borderBottomColor: '#F0F0F5',
  },

  modalTitle: {
    color: '#231c63',
    fontSize: 20,
    fontWeight: '800',
  },

  pickerList: {
    paddingHorizontal: 24,
    paddingTop: 16,
  },

  pickerItem: {
    backgroundColor: '#F8F6FF',
    borderRadius: 12,
    padding: 16,
    marginBottom: 10,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    borderWidth: 2,
    borderColor: '#E6E0FF',
  },

  pickerItemSelected: {
    backgroundColor: '#FFFFFF',
    borderColor: '#5F3BFF',
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.15,
    shadowRadius: 8,
    elevation: 3,
  },

  pickerItemText: {
    color: '#8A8FB0',
    fontSize: 16,
    fontWeight: '600',
  },

  pickerItemTextSelected: {
    color: '#5F3BFF',
    fontWeight: '800',
  },
});