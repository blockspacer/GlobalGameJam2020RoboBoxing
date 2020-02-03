﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FightController : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    [Header("Menu Buttons")]
    [SerializeField] private Button startButton;
    private GameController GameController;
    private float timer;
    public float delayTime;
    [SerializeField] private float attackTime = 0.3f;
    private bool attacking = false;
    private bool startRound = false;

    private int numHits_opponent;
    private int numHits_player;
    private int turn = 0;
    private bool lostGame = false;
    private bool opponentDefeated = false;

    [SerializeField] private Fighter opponent;
    [SerializeField] private Animator player_anim;
    [SerializeField] private Animator opponent_anim;

    public AudioClip BellSound;
    public float bellVolume;
    public float hitVolume = 0.5f;
    public float fightSoundDelay = 0;
    public bool endResult = false;

    [SerializeField] public AudioClip[] hitNoises;
    [SerializeField] public AudioClip[] voiceNoises;
    


    void Start()
    {
        GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        startButton.onClick.AddListener(StartButton);
        opponent = GameController.curOpponent;
        numHits_player = Random.Range(2,5);
        numHits_opponent = Random.Range(2,5);
        GameController.Instance.SetBackgroundMusic(GameController.Instance.FightPrepBackground,GameController.backgroundVolume,0);
    }

    void Update(){
        //set up a basic timer that activates when attacking becomes true;
        UpdateDelayTimer();
        if(delayTime <= 0){         
                if(startRound){
                    CalculateHits();
                }
            CheckConditions();
        }
    }

    void PlayHit(){
        int n = hitNoises.Length;
        int r = Random.Range(0,n-1);
        AudioSource.PlayClipAtPoint(hitNoises[r], transform.position, bellVolume);
        //play voices
        n = voiceNoises.Length;
        r = Random.Range(0,n-1);
        AudioSource.PlayClipAtPoint(voiceNoises[r], transform.position, bellVolume);
    }

    void Delay(float delay){
        delayTime = delay;
    }
    void UpdateDelayTimer(){
        delayTime -= Time.deltaTime;
    }

    void CalculateHits(){
        if(numHits_player <= 0)
            turn = 1;
        if(numHits_opponent <= 0)
            turn = 0;


        if(numHits_player > 0 && delayTime <= 0){
            int attack = 0;
            float damage = 0;
            damage =AttackArms(GameController.player,opponent);
            //Hit sounds go here
            numHits_player--;
            Delay(attackTime);
            turn = 1;
            if(attack > 1)
                player_anim.Play("player_punch");
            else if(attack > 3)
                player_anim.Play("player_jab");
            else
                player_anim.Play("player_swipe");
            if(hitNoises.Length > 0)
                PlayHit();
        }

        if(numHits_opponent > 0 && delayTime <= 0){
            //choose part to attack based on focus (set either randomly or by player)
            int attack = 0;
            float damage;
            damage = AttackArms(opponent,GameController.player);

            if(attack >= 3)
                opponent_anim.Play("enemy_jab");
            else
                opponent_anim.Play("badguy_swipe");
            if(hitNoises.Length > 0)
                PlayHit();
            numHits_opponent--;
            Delay(attackTime);
            turn = 0;
    }

    }
    private void CheckConditions(){
<<<<<<< Updated upstream

        if(!opponentDefeated && numHits_opponent == 0 && numHits_player == 0 && opponent.IsFighterLostByDamage() || opponent.IsFighterLostByScore(GameController.player) && GameController.round >= 6){
            opponent_anim.SetTrigger("badGuy_knockout");
            opponentDefeated = true;
            Delay(3);
        }
        if(!lostGame){
=======
        if(!endResult){
>>>>>>> Stashed changes
            if(numHits_opponent == 0 && numHits_player == 0 && GameController.IsPlayerWonFight()){
                GameController.ToggleWin(true);
                opponent_anim.SetTrigger("badGuy_knockout");
                Delay(3);
                endResult = true;
            }
            if(numHits_opponent == 0 && numHits_player == 0 && GameController.IsPlayerLostGame()){
                lostGame = true;
                player_anim.SetTrigger("player_knockout");
                GameController.ToggleWin(false);
                Delay(3);
                endResult = true;
            }
<<<<<<< Updated upstream
            
            if(numHits_opponent == 0 && numHits_player == 0 && delayTime <= 0){
                if(GameController.winSign.activeInHierarchy)
                    GameController.ToggleWin(true);
                if(GameController.loseSign.activeInHierarchy)
                    GameController.ToggleWin(false);
                GameController.round++;
                opponentDefeated = false;
=======
        }
        else if(delayTime <= 0 && numHits_opponent == 0 && numHits_player == 0){
            endResult = false;
            //Turn off any signs
            if(GameController.winSign.activeInHierarchy)
                GameController.ToggleWin(true);
            if(GameController.loseSign.activeInHierarchy)
                GameController.ToggleWin(false);

            if(GameController.IsPlayerWonFight()){
                GameController.PlayerWonFight();
                GameController.LoadRepairMenu();
            }
            else if(GameController.IsPlayerLostGame()){
                GameController.PlayerLostGame();
                
            }
            else{
                GameController.round++;
>>>>>>> Stashed changes
                GameController.curOpponent = opponent;
                GameController.LoadRepairMenu();
            }
        }

    }

    
    void StartButton(){
        startRound = true;
        startButton.gameObject.SetActive(false);
        startButton.enabled = false;
        var audioSrc = GameController.Instance.GetComponent<AudioSource>();
        audioSrc.volume = .2f;
        AudioSource.PlayClipAtPoint(BellSound, transform.position, bellVolume);
     }

    float AttackArms(Fighter attacker, Fighter defender){
        float damage = AttackCalclate(attacker,defender);
        defender.ModifyArm(damage);
        Debug.Log(attacker.name + " attacks " + defender.name + "'s arms and does " + (int)(-damage) + " damage!");
        return damage;
    }

    float AttackLegs(Fighter attacker, Fighter defender){
        float damage = AttackCalclate(attacker,defender);
        defender.ModifyLeg(damage);
        Debug.Log(attacker.name + " attacks " + defender.name + "'s legs and does " + (int)(-damage) + " damage!");
        return damage;
    }

    float AttackCables(Fighter attacker, Fighter defender){
        float damage = AttackCalclate(attacker,defender);
        defender.ModifyCableBox(damage);
        Debug.Log(attacker.name + " attacks " + defender.name + "'s cables and does " + (int)(-damage) + " damage!");
        return damage;
    }

    float AttackCircuits(Fighter attacker, Fighter defender){
        float damage = AttackCalclate(attacker,defender);
        defender.ModifyCircuitBoard(damage);
        Debug.Log(attacker.name + " attacks " + defender.name + "'s circuits and does " + (int)(-damage) + " damage!");
        return damage;
    }

    float AttackCoolant(Fighter attacker, Fighter defender){
        float damage = AttackCalclate(attacker,defender);
        defender.ModifyCoolant(damage);
        Debug.Log(attacker.name + " attacks " + defender.name + "'s coolant cell and does " + (int)(-damage) + " damage!");
        return damage;
    }

    float AttackEyes(Fighter attacker, Fighter defender){
        float damage = AttackCalclate(attacker,defender); 
        defender.ModifyEyes(damage);
        Debug.Log(attacker.name + " attacks " + defender.name + "'s eyes and does " + (int)(-damage) + " damage!");
        return damage;
    }

    private float AttackCalclate(Fighter attacker,Fighter defender){
        float accuracy = (0.01f*attacker.cableBox)+(0.01f*attacker.circuitBoard);
        accuracy = Mathf.Clamp(accuracy,0.01f,1);
        float damage = 5 * -(attacker.arm/defender.eyes + attacker.leg/defender.coolant) * accuracy;
        return damage;
    }

}