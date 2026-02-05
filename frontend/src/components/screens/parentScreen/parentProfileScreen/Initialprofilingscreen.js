import React, { useState } from 'react'
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  StatusBar,
  Alert,
  Animated
} from 'react-native'

import { useNavigation, useRoute } from '@react-navigation/native'
import { Ionicons } from '@expo/vector-icons'
import { LinearGradient } from 'expo-linear-gradient'

export default function InitialProfilingScreen() {
  const navigation = useNavigation()
  const route = useRoute()
  const { childName, ageGroup } = route.params

  const [currentQuestion, setCurrentQuestion] = useState(0)
  const [answers, setAnswers] = useState({})
  const [fadeAnim] = useState(new Animated.Value(1))

  const questions = [
    {
      id: 'has_allowance',
      question: 'Does your child receive a regular allowance or income?',
      icon: 'wallet-outline',
      color: '#FF6B9D',
      options: [
        { label: 'Yes', value: 'yes', emoji: '✅' },
        { label: 'No', value: 'no', emoji: '❌' }
      ]
    },
    {
      id: 'spending_pace',
      question: 'How does your child usually spend their allowance?',
      icon: 'time-outline',
      color: '#5F3BFF',
      options: [
        { label: 'Spends it right away', value: 'fast', emoji: '⚡' },
        { label: 'Spends it gradually', value: 'gradual', emoji: '📊' },
        { label: 'Saves part of it', value: 'saves', emoji: '💰' }
      ]
    },
    {
      id: 'spending_categories',
      question: 'What does your child usually spend their allowance on?',
      subtitle: '(Select all that apply)',
      icon: 'cart-outline',
      color: '#00D4AA',
      multiSelect: true,
      options: [
        { label: 'Food & drinks', value: 'food', emoji: '🍕' },
        { label: 'Entertainment (games, toys, movies)', value: 'entertainment', emoji: '🎮' },
        { label: 'Clothes & accessories', value: 'clothes', emoji: '👕' },
        { label: 'School supplies', value: 'school', emoji: '📚' }
      ]
    },
    {
      id: 'out_of_money_behavior',
      question: 'When your child runs out of allowance before the next payment, what do they usually do?',
      icon: 'alert-circle-outline',
      color: '#FFD93D',
      options: [
        { label: 'Ask for more allowance', value: 'ask_more', emoji: '🙏' },
        { label: 'Stop spending', value: 'stop', emoji: '🛑' },
        { label: 'Postpone purchases', value: 'postpone', emoji: '⏰' }
      ]
    },
    {
      id: 'tries_to_save',
      question: 'Does your child try to save part of their allowance?',
      icon: 'trending-up-outline',
      color: '#8B5CF6',
      options: [
        { label: 'Yes', value: 'yes', emoji: '✅' },
        { label: 'No', value: 'no', emoji: '❌' }
      ]
    },
    {
      id: 'money_mindset',
      question: "Which statement best describes your child's attitude toward money?",
      icon: 'bulb-outline',
      color: '#FF6B9D',
      options: [
        { label: 'Enjoys spending immediately', value: 'spender', emoji: '🎉' },
        { label: 'Balances spending and saving', value: 'balanced', emoji: '⚖️' },
        { label: 'Saves for the future', value: 'saver', emoji: '🎯' }
      ]
    },
    {
      id: 'feeling_after_spending',
      question: 'How does your child usually feel after spending money on non-essential items?',
      icon: 'happy-outline',
      color: '#00D4AA',
      options: [
        { label: 'Happy', value: 'happy', emoji: '😊' },
        { label: 'Regretful', value: 'regret', emoji: '😔' },
        { label: 'Neutral', value: 'neutral', emoji: '😐' }
      ]
    },
    {
      id: 'feeling_when_saving_grows',
      question: 'How does your child feel when they see their savings grow?',
      icon: 'stats-chart-outline',
      color: '#5F3BFF',
      options: [
        { label: 'Motivated', value: 'motivated', emoji: '💪' },
        { label: 'Proud', value: 'proud', emoji: '🌟' },
        { label: "Doesn't matter much", value: 'indifferent', emoji: '🤷' }
      ]
    },
    {
      id: 'reaction_to_100',
      question: 'If your child receives 100 EGP today, what would they do?',
      icon: 'cash-outline',
      color: '#FFD93D',
      options: [
        { label: 'Spend it all now on something fun', value: 'spend_all', emoji: '🎁' },
        { label: 'Spend part, save part', value: 'balanced', emoji: '💫' },
        { label: 'Save it all for a future goal', value: 'save_all', emoji: '🏆' }
      ]
    }
  ]

  const handleSelectAnswer = (questionId, value) => {
    const question = questions[currentQuestion]
    
    if (question.multiSelect) {
      // Handle multi-select
      const currentAnswers = answers[questionId] || []
      const newAnswers = currentAnswers.includes(value)
        ? currentAnswers.filter(v => v !== value)
        : [...currentAnswers, value]
      
      setAnswers({ ...answers, [questionId]: newAnswers })
    } else {
      // Handle single select
      setAnswers({ ...answers, [questionId]: value })
    }
  }

  const handleNext = () => {
    const question = questions[currentQuestion]
    const answer = answers[question.id]
    
    // Validate answer
    if (!answer || (question.multiSelect && answer.length === 0)) {
      Alert.alert('Required', 'Please select an answer before continuing')
      return
    }

    if (currentQuestion < questions.length - 1) {
      // Fade out animation
      Animated.timing(fadeAnim, {
        toValue: 0,
        duration: 200,
        useNativeDriver: true,
      }).start(() => {
        setCurrentQuestion(currentQuestion + 1)
        // Fade in animation
        Animated.timing(fadeAnim, {
          toValue: 1,
          duration: 200,
          useNativeDriver: true,
        }).start()
      })
    } else {
      handleComplete()
    }
  }

  const handlePrevious = () => {
    if (currentQuestion > 0) {
      Animated.timing(fadeAnim, {
        toValue: 0,
        duration: 200,
        useNativeDriver: true,
      }).start(() => {
        setCurrentQuestion(currentQuestion - 1)
        Animated.timing(fadeAnim, {
          toValue: 1,
          duration: 200,
          useNativeDriver: true,
        }).start()
      })
    }
  }

  const handleComplete = () => {
    // Generate login code
    const loginCode = childName.substring(0, 3).toUpperCase() + Math.floor(Math.random() * 9000 + 1000)

    // Calculate profile features
    const profileData = {
      childName,
      ageGroup,
      loginCode,
      answers,
      // Derived features
      child_age_group: ageGroup,
      has_allowance: answers.has_allowance === 'yes',
      spending_pace: answers.spending_pace,
      tries_to_save: answers.tries_to_save === 'yes',
      spending_categories: answers.spending_categories || [],
      out_of_money_behavior: answers.out_of_money_behavior,
      money_mindset: answers.money_mindset,
      feeling_after_spending: answers.feeling_after_spending,
      feeling_when_saving_grows: answers.feeling_when_saving_grows,
      reaction_to_100: answers.reaction_to_100,
      // Additional derived metrics
      impulse_spending_tendency: calculateImpulseTendency(answers),
      self_control_level: calculateSelfControl(answers),
      saving_tendency: calculateSavingTendency(answers),
      motivation_level: calculateMotivation(answers),
    }

    // Show success message
    Alert.alert(
      'Profile Created! 🎉',
      `${childName}'s login code is: ${loginCode}\n\nPlease save this code. ${childName} will use it to access their account.`,
      [
        {
          text: 'OK',
          onPress: () => {
            // Navigate back to Manage Children with new child data
            navigation.navigate('ManageChildren', { newChild: profileData })
          }
        }
      ]
    )
  }

  // Helper functions to calculate derived features
  const calculateImpulseTendency = (answers) => {
    const impulsiveFactors = [
      answers.spending_pace === 'fast',
      answers.out_of_money_behavior === 'ask_more',
      answers.money_mindset === 'spender',
      answers.reaction_to_100 === 'spend_all'
    ]
    return impulsiveFactors.filter(Boolean).length / impulsiveFactors.length
  }

  const calculateSelfControl = (answers) => {
    const controlFactors = [
      answers.spending_pace === 'saves',
      answers.out_of_money_behavior === 'postpone',
      answers.tries_to_save === 'yes',
      answers.money_mindset === 'saver'
    ]
    return controlFactors.filter(Boolean).length / controlFactors.length
  }

  const calculateSavingTendency = (answers) => {
    const savingFactors = [
      answers.tries_to_save === 'yes',
      answers.money_mindset === 'saver' || answers.money_mindset === 'balanced',
      answers.reaction_to_100 === 'save_all' || answers.reaction_to_100 === 'balanced',
      answers.feeling_when_saving_grows !== 'indifferent'
    ]
    return savingFactors.filter(Boolean).length / savingFactors.length
  }

  const calculateMotivation = (answers) => {
    return answers.feeling_when_saving_grows === 'motivated' ? 'high' :
           answers.feeling_when_saving_grows === 'proud' ? 'medium' : 'low'
  }

  const progress = ((currentQuestion + 1) / questions.length) * 100
  const question = questions[currentQuestion]
  const currentAnswer = answers[question.id]

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
            <Text style={styles.headerTitleText}>Financial Personality</Text>
            <Text style={styles.headerSubtitle}>
              Question {currentQuestion + 1} of {questions.length}
            </Text>
          </View>

          <View style={{ width: 40 }} />
        </View>

        {/* Progress Bar */}
        <View style={styles.progressContainer}>
          <View style={styles.progressBar}>
            <Animated.View 
              style={[
                styles.progressFill, 
                { width: `${progress}%` }
              ]} 
            />
          </View>
          <Text style={styles.progressText}>{Math.round(progress)}% Complete</Text>
        </View>
      </LinearGradient>

      {/* Question Content */}
      <ScrollView 
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        <Animated.View style={{ opacity: fadeAnim }}>
          {/* Question Card */}
          <View style={styles.questionCard}>
            <View style={[styles.questionIconContainer, { backgroundColor: `${question.color}15` }]}>
              <Ionicons name={question.icon} size={32} color={question.color} />
            </View>

            <Text style={styles.questionText}>{question.question}</Text>
            
            {question.subtitle && (
              <Text style={styles.questionSubtitle}>{question.subtitle}</Text>
            )}
          </View>

          {/* Options */}
          <View style={styles.optionsContainer}>
            {question.options.map((option) => {
              const isSelected = question.multiSelect
                ? currentAnswer?.includes(option.value)
                : currentAnswer === option.value

              return (
                <TouchableOpacity
                  key={option.value}
                  style={[
                    styles.optionCard,
                    isSelected && [styles.optionCardSelected, { borderColor: question.color }]
                  ]}
                  onPress={() => handleSelectAnswer(question.id, option.value)}
                  activeOpacity={0.7}
                >
                  <View style={styles.optionLeft}>
                    <Text style={styles.optionEmoji}>{option.emoji}</Text>
                    <Text style={[
                      styles.optionText,
                      isSelected && styles.optionTextSelected
                    ]}>
                      {option.label}
                    </Text>
                  </View>

                  <View style={[
                    styles.checkbox,
                    isSelected && [styles.checkboxSelected, { backgroundColor: question.color }]
                  ]}>
                    {isSelected && (
                      <Ionicons name="checkmark" size={18} color="#FFFFFF" />
                    )}
                  </View>
                </TouchableOpacity>
              )
            })}
          </View>

          {/* Helper Text */}
          <View style={styles.helperCard}>
            <Ionicons name="information-circle-outline" size={20} color="#5F3BFF" />
            <Text style={styles.helperText}>
              {question.multiSelect 
                ? 'You can select multiple options'
                : 'Select the option that best describes your child'}
            </Text>
          </View>
        </Animated.View>
      </ScrollView>

      {/* Navigation Buttons */}
      <View style={styles.buttonContainer}>
        <View style={styles.buttonRow}>
          {currentQuestion > 0 && (
            <TouchableOpacity 
              style={styles.previousButton}
              onPress={handlePrevious}
            >
              <Ionicons name="arrow-back" size={20} color="#5F3BFF" />
              <Text style={styles.previousText}>Previous</Text>
            </TouchableOpacity>
          )}

          <TouchableOpacity 
            style={[styles.nextButton, currentQuestion === 0 && { flex: 1 }]}
            onPress={handleNext}
          >
            <LinearGradient
              colors={['#5F3BFF', '#3B1DFF', '#7C4DFF']}
              start={{ x: 0, y: 0 }}
              end={{ x: 1, y: 1 }}
              style={styles.nextGradient}
            >
              <Text style={styles.nextText}>
                {currentQuestion === questions.length - 1 ? 'Complete' : 'Next'}
              </Text>
              <Ionicons 
                name={currentQuestion === questions.length - 1 ? 'checkmark' : 'arrow-forward'} 
                size={20} 
                color="#FFFFFF" 
              />
            </LinearGradient>
          </TouchableOpacity>
        </View>
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
    paddingBottom: 20,
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
    marginBottom: 16,
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
    fontSize: 20,
    fontWeight: '800',
    letterSpacing: 0.5,
  },

  headerSubtitle: {
    color: 'rgba(255, 255, 255, 0.9)',
    fontSize: 13,
    fontWeight: '600',
    marginTop: 2,
  },

  progressContainer: {
    paddingHorizontal: 24,
  },

  progressBar: {
    height: 6,
    backgroundColor: 'rgba(255, 255, 255, 0.3)',
    borderRadius: 3,
    overflow: 'hidden',
    marginBottom: 6,
  },

  progressFill: {
    height: '100%',
    backgroundColor: '#FFD93D',
    borderRadius: 3,
  },

  progressText: {
    color: '#FFFFFF',
    fontSize: 12,
    fontWeight: '700',
    textAlign: 'center',
  },

  /* ================= SCROLL VIEW ================= */
  scrollView: {
    flex: 1,
  },

  scrollContent: {
    padding: 24,
    paddingBottom: 200,
  },

  /* ================= QUESTION CARD ================= */
  questionCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 24,
    padding: 24,
    marginBottom: 24,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.08,
    shadowRadius: 12,
    elevation: 4,
    alignItems: 'center',
  },

  questionIconContainer: {
    width: 70,
    height: 70,
    borderRadius: 35,
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 16,
  },

  questionText: {
    color: '#231c63',
    fontSize: 18,
    fontWeight: '800',
    textAlign: 'center',
    lineHeight: 26,
  },

  questionSubtitle: {
    color: '#8A8FB0',
    fontSize: 14,
    fontWeight: '600',
    textAlign: 'center',
    marginTop: 8,
  },

  /* ================= OPTIONS ================= */
  optionsContainer: {
    gap: 12,
    marginBottom: 20,
  },

  optionCard: {
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    padding: 18,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    borderWidth: 2,
    borderColor: '#E6E0FF',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 8,
    elevation: 2,
  },

  optionCardSelected: {
    backgroundColor: '#F8F6FF',
    borderWidth: 2,
    shadowColor: '#5F3BFF',
    shadowOpacity: 0.15,
  },

  optionLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    flex: 1,
  },

  optionEmoji: {
    fontSize: 28,
    marginRight: 14,
  },

  optionText: {
    color: '#8A8FB0',
    fontSize: 15,
    fontWeight: '600',
    flex: 1,
  },

  optionTextSelected: {
    color: '#231c63',
    fontWeight: '800',
  },

  checkbox: {
    width: 28,
    height: 28,
    borderRadius: 14,
    borderWidth: 2,
    borderColor: '#C0C0D0',
    justifyContent: 'center',
    alignItems: 'center',
  },

  checkboxSelected: {
    borderWidth: 0,
  },

  /* ================= HELPER CARD ================= */
  helperCard: {
    backgroundColor: '#F0ECFF',
    borderRadius: 12,
    padding: 14,
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: '#E6E0FF',
  },

  helperText: {
    flex: 1,
    marginLeft: 10,
    color: '#5F3BFF',
    fontSize: 13,
    fontWeight: '600',
    lineHeight: 18,
  },

  /* ================= BUTTONS ================= */
  buttonContainer: {
    position: 'absolute',
    bottom: 10,
    left: 0,
    right: 0,
    padding: 24,
    paddingBottom: 100,
    backgroundColor: '#F4F6FB',
    borderTopWidth: 1,
    borderTopColor: '#E6E0FF',
  },

  buttonRow: {
    flexDirection: 'row',
    gap: 12,
  },

  previousButton: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    paddingVertical: 20,
    borderWidth: 2,
    borderColor: '#5F3BFF',
    gap: 8,
  },

  previousText: {
    color: '#5F3BFF',
    fontSize: 16,
    fontWeight: '800',
  },

  nextButton: {
    flex: 2,
    borderRadius: 16,
    overflow: 'hidden',
    paddingVertical: 0,
    shadowColor: '#5F3BFF',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 12,
    elevation: 6,
  },

  nextGradient: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 22,
    gap: 8,
  },

  nextText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '800',
  },
});