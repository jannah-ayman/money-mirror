      //  const expenseData = {
      //    cost: parseFloat(cost),
      //    category: selectedCategory,
      //    itemName: selectedCategory === 'other' ? otherCategory : getCategoryName(selectedCategory),
      //    mood: selectedMood,
      //    timestamp: new Date().toISOString()
      //  };

      //  try {
      //    const response = await fetch('https: your-api-endpoint.com/expenses', {
      //      method: 'POST',
      //      headers: {
      //        'Content-Type': 'application/json',
      //      },
      //      body: JSON.stringify(expenseData),
      //    });

      //    if (response.ok) {
      //      alert('Space mission added successfully!');
      //      setCost('');
      //      setSelectedCategory(null);
      //      setOtherCategory('');
      //      setSelectedMood(null);
      //    } else {
      //      alert('Something went wrong. Please try again!');
      //    }
      //  } catch (error) {
      //   //  console.error('Error submitting expense:', error);
      //    alert('Connection lost in space !');
      //  } finally {
      //  }


import { View, Text, ImageBackground, Image, TouchableOpacity,TextInput,ScrollView } from 'react-native';
import { Provider as PaperProvider } from 'react-native-paper';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import AntDesign from '@expo/vector-icons/AntDesign';
import Svg, { Path } from 'react-native-svg';
import styles from "./styles";
import { useNavigation, useRoute } from '@react-navigation/native';
import { LinearGradient } from 'expo-linear-gradient';
import { useState } from 'react';
import Toast from 'react-native-toast-message';
import { Dimensions } from 'react-native';
// import CustomStatusBar from '../../../../../CustomStatusBar';


const { width } = Dimensions.get('window');

export default function ChildExpenseLogScreen() {
  const { goBack } = useNavigation();

  const [cost, setCost] = useState('');
  const [thing,setThing] = useState('')
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [otherCategory, setOtherCategory] = useState('');
  const [selectedMood, setSelectedMood] = useState(null);
  
  const route = useRoute();
  const child = route.params?.child ?? {};
  const {recentPurchases} = child;

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

  // const recentPurchases = [
    // { id: 1, name: 'Toy Car', cost: '$15.99', category: 'toys', mood: 'excited' },
    // { id: 2, name: 'Ice Cream', cost: '$4.50', category: 'food', mood: 'happy' },
    // { id: 3, name: 'Story Book', cost: '$12.00', category: 'books', mood: 'happy' }
  // ];

  const handleCategorySelect = (id) => {
    setSelectedCategory(id);
    if (id !== 'other') setOtherCategory('');
  };

  const handleSubmit = () => {
    if (!cost || !selectedCategory || !selectedMood) {
      Toast.show({
        type: 'error',
        text2: 'Please complete all mission details 🚀',
        text2Style: { color: 'black', fontSize: 16 },
        position: 'top',
      });
      return;
    }

    if (selectedCategory === 'other' && !otherCategory) {
      Toast.show({
        type: 'error',
        text2: 'Tell us what you discovered ',
        text2Style: { color: 'black', fontSize: 16 },
        position: 'top',
      });
      return;
    }

    Toast.show({
      type: 'success',
      text2: 'Mission logged successfully ',
      text2Style: { color: 'black', fontSize: 16 },
      position: 'top',
    });

    setCost('');
    setSelectedCategory(null);
    setOtherCategory('');
    setSelectedMood(null);
  };

  const getItemById = (list, id, key) =>
    list.find(item => item.id === id)?.[key] ?? '';

  return (
    <LinearGradient colors={['#F4F6FF', '#EEF1FF', '#E9EDFF']} style={{ flex: 1,paddingBottom:80}}>
     <KeyboardAwareScrollView style={{ flex: 1 }}
      contentContainerStyle={{ flexGrow: 1 }}
      enableOnAndroid 
      keyboardShouldPersistTaps="handled" >
      {/* <ScrollView
        contentContainerStyle={{
          flexGrow: 1,
        }}
        keyboardShouldPersistTaps="handled"
        overScrollMode="never"
        showsVerticalScrollIndicator={false}
      > */}
      {/* <CustomStatusBar barStyle="dark-content"/> */}
      <View style={styles.headerContainer}> 
        <ImageBackground source={require('../../../../../assets/images/download (3).jpg')} style={styles.headerBackground} resizeMode="cover"> 
         <AntDesign name="arrow-left" size={24} color="white" onPress={goBack} style={styles.backButton}/>
          <View style={styles.intro}>
           <View style={styles.astr}>
            <Image source={require('../../../../../assets/images/star.png')} style={{width:60,height:60,marginTop:-20,marginInlineStart:50}}/>
            <Image source={require('../../../../../assets/images/7300_7_2_10.png')} style={{width:230,height:230,marginInlineStart:-30,marginTop:-20}}/>
          </View>
          <View style={styles.msgIntro}>
            <Image source={require('../../../../../assets/images/spendora.png')} style={{width:100,height:100,marginInlineStart:30}}/> 
            <Text style={styles.msgText}>Welcome to Spendora Planet! Your shopping adventure in space starts now</Text>
          </View>
          </View>
        </ImageBackground>
        <View style={styles.waveContainer}>
          <Svg 
            height="120" 
            width={width} 
            viewBox={`0 0 ${width} 120`}
            style={styles.wave}
            preserveAspectRatio="none"
          >
            <Path
              d={`M 0 60 Q ${width * 0.125} 30 ${width * 0.25} 60 T ${width * 0.5} 60 T ${width * 0.75} 60 T ${width} 60 L ${width} 120 L 0 120 Z`}
              fill="#EEF1FF"
              opacity="0.5"
            />
          </Svg>
          <Svg 
            height="120" 
            width={width} 
            viewBox={`0 0 ${width} 120`}
            style={[styles.wave, styles.waveFront]}
            preserveAspectRatio="none"
          >
            <Path
              d={`M 0 70 Q ${width * 0.125} 45 ${width * 0.25} 70 T ${width * 0.5} 70 T ${width * 0.75} 70 T ${width} 70 L ${width} 120 L 0 120 Z`}
              fill="#EEF1FF"
            />
          </Svg>
        </View>
      </View>


        <View style={styles.formContainer}>
          <Text style={styles.MianTitle}>Add a Space Mission </Text>

          <View style={styles.inputContainer}>
          <View style={{flexDirection:'row'}}>
          <Image source={require('../../../../../assets/images/coin.png')} style={{width:30,height:30,marginInlineEnd:10}}/>
            <Text style={styles.label}>How much did this mission cost?</Text>
          </View>
            <TextInput
              style={styles.costInput}
              placeholder="0.00 $"
              placeholderTextColor="#999"
              keyboardType="decimal-pad"
              value={cost}
              onChangeText={setCost}
              includeFontPadding={false}
              underlineColorAndroid="transparent"
            />
          </View>

          <View style={styles.inputContainer}>
          <View style={{flexDirection:'row'}}>
          <Image source={require('../../../../../assets/images/shopping-basket.png')} style={{width:30,height:30,marginInlineEnd:10}}/>
            <Text style={styles.label2}>What did you pick from the galaxy ?</Text>
          </View>
            <TextInput
              style={styles.ThingInput}
              placeholder="Tell us what you picked"
              placeholderTextColor="#999"
              value={thing}
              onChangeText={setThing}
              includeFontPadding={false}
              underlineColorAndroid="transparent"
            />
          </View>

          <View style={styles.section}>
          <View style={{flexDirection:'row'}}>
          <Image source={require('../../../../../assets/images/categories.png')} style={{width:30,height:30,marginInlineEnd:10}}/>
            <Text style={styles.label}>What did you buy?</Text>
          </View>
            <View style={styles.categoriesGrid}>
              {categories.map(category => (
                <TouchableOpacity
                  key={category.id}
                  style={[
                    styles.categoryCard,
                    selectedCategory === category.id && styles.categoryCardSelected
                  ]}
                  onPress={() => handleCategorySelect(category.id)}
                >
                  <Image source={category.icon} style={styles.categoryIcon} />
                  <Text style={styles.categoryName}>{category.name}</Text>
                </TouchableOpacity>
              ))}

              {selectedCategory === 'other' && (
              <TextInput
                style={[
                  styles.textInput,
                  otherCategory ? styles.textInput : styles.placeholderText
                ]}
                placeholder="What did you discover on your mission? "
                placeholderTextColor="#999"
                value={otherCategory}
                onChangeText={setOtherCategory}
              />
            )}
            </View>
          </View>

          

          <View style={styles.section}>
          <View style={{flexDirection:'row'}}>
          <Image source={require('../../../../../assets/images/sentiment-analysis.png')} style={{width:30,height:30,marginInlineEnd:10}}/>
            <Text style={styles.label}>How do you feel about it?</Text>
          </View>
            <View style={styles.moodContainer}>
              {moods.map(mood => (
                <TouchableOpacity
                  key={mood.id}
                  style={[
                    styles.moodChip,
                    selectedMood === mood.id && styles.moodChipSelected
                  ]}
                  onPress={() => setSelectedMood(mood.id)}
                >
                  <Image source={mood.icon} style={styles.moodIcon} />
                  <Text style={styles.categoryName}>{mood.name}</Text>
                </TouchableOpacity>
              ))}
            </View>
          </View>
        

        <LinearGradient colors={['#2D1B4E', '#4B2C82', '#7B5FC9']}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.btn}
        >
          <TouchableOpacity onPress={handleSubmit}>
            <Text style={styles.btnText}>Add Mission </Text>
          </TouchableOpacity>
        </LinearGradient>
        </View>

              
        {/* <View style={styles.recentSection}>
          <LinearGradient 
            colors={['#2D1B4E', '#4B2C82', '#7B5FC9']}
            // colors={['#7c509b', '#f7a9c9']}
            start={{ x: 0, y: 0 }}
            end={{ x: 1, y: 1 }}
            style={{
              paddingInline:15,
              paddingBlock:15,
              borderRadius:30,
            }}
            >
          <View style={{flexDirection:'row'}}>
          <Image source={require('../../../../../assets/images/history (1).png')} style={{width:40,height:40,marginInlineEnd:10}}/>
            <Text style={styles.recentTitle}>Recent Missions</Text>
          </View>
              {recentPurchases.length === 0 ? (
                <View style={styles.emptyPurchasesContainer}>
                  <Text style={styles.emptyPurchasesText}>
                    No recent purchases 
                  </Text> 
                </View>
              ) : (
                recentPurchases.slice(0, 2).map(purchase => (
                  <View key={purchase.id} style={styles.purchaseCard}>
                    <View>
                      <Text style={styles.purchaseName}>{purchase.name}</Text>
                      <Text style={styles.purchaseCost}>{purchase.cost}</Text>
                    </View>
                    <View style={{ flexDirection: 'row' }}>
                      <Image source={getItemById(categories, purchase.category, 'icon')} style={styles.purchaseIcon} />
                      <Image source={getItemById(moods, purchase.mood, 'icon')} style={styles.purchaseIcon} />
                    </View>
                  </View>
                ))
              )}
        </LinearGradient>
        </View> */}
      {/* </ScrollView>  */}
      </KeyboardAwareScrollView>
    </LinearGradient>
  );
}


//['#2D1B4E', '#4B2C82', '#7B5FC9']