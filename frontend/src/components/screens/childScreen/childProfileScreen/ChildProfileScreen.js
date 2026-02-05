import { View, Text, ImageBackground, Image, TouchableOpacity, Modal } from 'react-native';
import { UseChildStore } from '../../../../store/UseChildStore';
import { useFocusEffect } from '@react-navigation/native';
import { useNavigation } from '@react-navigation/native';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import { LinearGradient } from 'expo-linear-gradient';
import AntDesign from '@expo/vector-icons/AntDesign';
import styles from "./styles";
import { useState, useCallback } from 'react';

export default function ChildProfileScreen() {

  const { goBack } = useNavigation();
  const child = UseChildStore(state => state.currentChild);
  const [name, setName] = useState('');
  const [type, setType] = useState('');
  const [typeDesc, setTypeDesc] = useState('');
  const [personality, setPersonality] = useState('');
  const [insights, setInsights] = useState([]);

  const avatars = [
    { id: '1', name: 'Nova', image: require("../../../../../assets/images/nova r.png") },
    { id: '2', name: 'Luna', image: require("../../../../../assets/images/luna.png") },
    { id: '3', name: 'Cosmo', image: require("../../../../../assets/images/cusmo.png") },
    { id: '4', name: 'Aura', image: require("../../../../../assets/images/aura.png") },
  ];

  const [selectedAvatar, setSelectedAvatar] = useState(avatars[0]);
  const [showModal, setShowModal] = useState(false);

  useFocusEffect(
    useCallback(() => {
      if (child) {
        setName(child.name || '');
        setType(child.type || '');
        setTypeDesc(child.typeDesc || '');
        setPersonality(child.personality || '');
        setInsights(child.insights || '')
        
        const foundAvatar = avatars.find(a => a.name === child.avatar);
        setSelectedAvatar(foundAvatar || avatars[0]);
      } else {
        setName('');
        setType('');
        setTypeDesc('');
        setPersonality('');
        setSelectedAvatar(avatars[0]);
      }
    }, [child])
  );

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

  return (
    <View style={{ flex: 1,marginBottom:80 }}>
      <KeyboardAwareScrollView
        contentContainerStyle={{ flexGrow: 1 }}
        enableOnAndroid
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.headerContainer}>
          <ImageBackground source={require('../../../../../assets/images/download (45).jpg')} style={styles.headerBackground}>
            <AntDesign name="arrow-left" size={24} color="white" onPress={goBack} style={styles.backButton}/>
            <View style={styles.profile}>
              <Image source={selectedAvatar.image} style={styles.imgprofile}/>
            </View>
          </ImageBackground>
          <View style={styles.curve} />
        </View>

        <LinearGradient
          colors={['#4B2C8C', '#3552B3']}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.btn}
        >
          <TouchableOpacity onPress={() => setShowModal(true)}>
            <Text style={styles.btnText}>Change Avatar</Text>
          </TouchableOpacity>
        </LinearGradient>

        <Modal
          animationType="slide"
          transparent={true}
          visible={showModal}
          onRequestClose={() => setShowModal(false)}
          style={{flex:1}}
        >
          <View style={styles.modalBackground}>
            <View style={styles.popup}>
              <Text style={styles.popupTitle}>Choose your Avatar</Text>

              <View style={styles.grid}>
                {avatars.map(avatar => (
                  <TouchableOpacity
                    key={avatar.id}
                    style={styles.gridItem}
                    onPress={() => {
                      setSelectedAvatar(avatar);
                      setShowModal(false);
                    }}
                  >
                    <Image source={avatar.image} style={styles.gridImage} />
                    <Text style={styles.gridText}>{avatar.name}</Text>
                  </TouchableOpacity>
                ))}
              </View>

              <TouchableOpacity
                style={styles.cancelBtn}
                onPress={() => setShowModal(false)}
              >
                <Text style={styles.cancelText}>Cancel</Text>
              </TouchableOpacity>
            </View>
          </View>
        </Modal>

        <View style={styles.Intro}>
          <Text style={styles.IntroText}>
            Hi {name} the {type} {'\n'}
          </Text>
        </View>

        <View style={styles.personality}>
            <View>
              <Image source={require("../../../../../assets/images/Cute_astronaut_standing_on_moon_holding_teddy_bear_and_flag_cartoon_vector_icon_illustration_science___Premium_Vector-removebg-preview.png")} style={styles.imgPerson}/>
            </View>
            <View style={styles.personText}>
              <Text style={styles.TextP}>
                We went on a fun trip through your galaxy {"\n"}
                Now you’re back on your own planet.
                From our journey together, I learned who you are — {typeDesc}
              </Text>
            </View>
        </View>

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

          {showAdv(insights)}
        </LinearGradient>
        </View>

        <LinearGradient
          colors={['#4B2C8C', '#3552B3']}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
          style={styles.btn2}
        >
          <TouchableOpacity onPress={() => setShowModal(true)} style={{flexDirection:'row'}}>
            <Text style={styles.btnText2}>End Space Trip</Text>
            {/* <Image
              source={require("../../../../../assets/images/starss.gif")}
              style={styles.advantagesIcon}
            /> */}
          </TouchableOpacity>
        </LinearGradient>
      </KeyboardAwareScrollView>
    </View>
  );
}
