import { View, Text, Image, ImageBackground, TouchableOpacity } from 'react-native';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import { useFocusEffect } from '@react-navigation/native';
import { useCallback, useState } from 'react';
import { useNavigation } from '@react-navigation/native';
import AntDesign from '@expo/vector-icons/AntDesign';
import { LinearGradient } from 'expo-linear-gradient';
import { Picker } from '@react-native-picker/picker';
import { UseChildStore } from '../../../../store/UseChildStore';
import styles from './styles';

export default function ChildHistoryScreen() {

  const { goBack } = useNavigation();
  const [category, setCategory] = useState('all');
  const [time, setTime] = useState('week');
  const [purchases, setPurchases] = useState([]);

  const child = UseChildStore(state => state.currentChild);

  useFocusEffect(
    useCallback(() => {
      if (child?.purchases) {
        setPurchases(child.purchases);
      } else {
        setPurchases([]);
      }
    }, [child])
  );

  const totalItems = purchases.length;

  const totalCost = purchases.reduce((acc, item) => {
    const costNum = parseFloat(item.cost.replace('$','')) || 0;
    return acc + costNum;
  }, 0);

  const moodCount = {};
  purchases.forEach(p => {
    moodCount[p.mood] = (moodCount[p.mood] || 0) + 1;
  });
  let mostUsedMood = '';
  let maxCount = 0;
  for (let mood in moodCount) {
    if (moodCount[mood] > maxCount) {
      maxCount = moodCount[mood];
      mostUsedMood = mood;
    }
  }
  const categories = [
    { id: 'toys', name: 'Toys', icon: require('../../../../../assets/images/toys.png') },
    { id: 'food', name: 'Food', icon: require('../../../../../assets/images/snack.png') },
    { id: 'books', name: 'Books', icon: require('../../../../../assets/images/stack-of-books.png') },
    { id: 'games', name: 'Games', icon: require('../../../../../assets/images/game-console.png') },
    { id: 'clothes', name: 'Clothes', icon: require('../../../../../assets/images/laundry.png') },
    { id: 'other', name: 'Other', icon: require('../../../../../assets/images/question-mark.png') }
  ];

  const moods = [
    { id: 'happy', name: 'Happy', icon: require('../../../../../assets/images/winking-face (1).png') },
    { id: 'sad', name: 'Sad', icon: require('../../../../../assets/images/crying.png') },
    { id: 'excited', name: 'Excited', icon: require('../../../../../assets/images/laugh.png') },
    { id: 'okay', name: 'Okay', icon: require('../../../../../assets/images/winking-face.png') },
    { id: 'worried', name: 'Worried', icon: require('../../../../../assets/images/sad-face.png') }
  ];

  const getItemById = (list, id, key) =>
    list.find(item => item.id === id)?.[key] ?? '';

  return (
    <KeyboardAwareScrollView
      contentContainerStyle={{ flexGrow: 1 }}
      enableOnAndroid
      keyboardShouldPersistTaps="handled"
    >
      <View style={{ flex: 1 }}>
        {/* Header */}
        <ImageBackground
          source={require('../../../../../assets/images/download (3).jpg')}
          style={styles.headerContainer}
          resizeMode="cover"
        >
          <TouchableOpacity onPress={goBack} style={styles.backButton}>
            <AntDesign name="arrow-left" size={24} color="white" />
          </TouchableOpacity>

          <View style={styles.mainHeader}>
            <Image source={require('../../../../../assets/images/read5.png')} style={styles.img1}/>
            <Image source={require('../../../../../assets/images/msg4.png')} style={styles.img2}/>
          </View>
        </ImageBackground>

        {/* Filter Card */}
        <View style={styles.FilterCard}>
          <View style={{ flexDirection: 'row', alignItems: 'center' }}>
            <Image source={require('../../../../../assets/images/filter.png')} style={styles.img3}/>
            <Text style={styles.title}>Filter Your Missions</Text>
          </View>

          <View style={styles.row}>
            <View style={styles.dropdown}>
              <Picker
                selectedValue={category}
                onValueChange={(value) => setCategory(value)}
              >
                <Picker.Item label="All Categories" value="all" />
                <Picker.Item label="Food" value="food" />
                <Picker.Item label="Games" value="games" />
              </Picker>
            </View>

            <View style={styles.dropdown}>
              <Picker
                selectedValue={time}
                onValueChange={(value) => setTime(value)}
              >
                <Picker.Item label="This Week" value="week" />
                <Picker.Item label="This Month" value="month" />
                <Picker.Item label="This Year" value="year" />
              </Picker>
            </View>
          </View>
        </View>


        <View style={styles.container}>
          <LinearGradient colors={['#4B2C82', '#7B5FC9']} style={styles.card}>
            <Text style={styles.cardTitle}>All Missions</Text>
            <Text style={styles.cardValue}>{totalItems}</Text>
          </LinearGradient>

          <LinearGradient colors={[ '#4B2C82', '#7B5FC9']} style={styles.card}>
            <Text style={styles.cardTitle}>Total Spent</Text>
            <Text style={styles.cardValue}>${totalCost.toFixed(2)}</Text>
          </LinearGradient>

          <LinearGradient colors={['#4B2C82', '#7B5FC9']} style={styles.card}>
            <Text style={styles.cardTitle}>Top Mood</Text>
            {mostUsedMood ? (
              <Image 
                source={getItemById(moods, mostUsedMood, 'icon')} 
                style={{ width: 50, height: 50, resizeMode: 'contain',marginTop:5 }} 
              />
            ) : (
              <Text style={styles.cardValue}>–</Text>
            )}
          </LinearGradient>
        </View>

        {/*Purchases */}
        <View style={styles.recentSection}>
          <LinearGradient
            colors={[ '#4B2C82', '#7B5FC9']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
            style={{
              paddingHorizontal: 15,
              paddingVertical: 15,
              borderRadius: 30,
            }}
          >
            <View style={{ flexDirection: 'row', alignItems: 'center', marginBottom: 10 }}>
              <Image source={require('../../../../../assets/images/history (1).png')} style={{ width: 40, height: 40, marginRight: 10 }}/>
              <Text style={styles.recentTitle}>Your Missions</Text>
            </View>

            {purchases.length === 0 ? (
              <View style={styles.emptyPurchasesContainer}>
                <Text style={styles.emptyPurchasesText}>No purchases</Text>
              </View>
            ) : (
              purchases.map(purchase => (
                <View key={purchase.id} style={styles.purchaseCard}>
                  <View style={{ flexDirection: 'row', alignItems: 'center', flex: 1 }}>

                    <Image
                      source={getItemById(categories, purchase.category, 'icon')}
                      style={styles.purchaseIcon}
                    />
                    <View>
                      <Text style={styles.purchaseName}>{purchase.name}</Text>
                      <Text style={styles.purchaseDate}>
                        {purchase.date}
                      </Text>
                    </View>

                  </View>
                  <View style={{ alignItems: 'center' }}>
                    <Text style={styles.purchaseCost}>{purchase.cost}</Text>
                    <Image
                      source={getItemById(moods, purchase.mood, 'icon')}
                      style={styles.purchaseIcon}
                    />
                  </View>

                </View>

              ))
            )}
          </LinearGradient>
        </View>
      </View>
    </KeyboardAwareScrollView>
  );
}
