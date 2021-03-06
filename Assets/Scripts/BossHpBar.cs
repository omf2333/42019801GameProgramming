﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHpBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public GameObject boss;
    public Room bossRoom;

    private bool isBossBorn = false;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    public void Start()
    {

    }
    public void Update()
    {
        //Debug.Log(CameraController.instance.currRoom.name);
        //Debug.Log(SceneManager.GetActiveScene().name);
        //Debug.Log(SceneManager.GetSceneByName("Basement8").GetRootGameObjects());
        if (bossRoom == null)
        {
            //Debug.Log(RoomController.instance.loadedRooms);
            bossRoom = RoomController.instance.loadedRooms.Find(delegate (Room room)
            {
                return room.name.Contains("End");
            });
        }
        if (CameraController.instance.currRoom == bossRoom)
        {
            if(boss == null)
            {
                Debug.Log("BOSS");
                boss = GameObject.FindObjectOfType<Boss>().gameObject; 
                //boss = GameObject.Find("BOSS1(Clone)");
                if (boss != null)
                {
                    Debug.Log("BOSSHP");
                    SetMaxHealth(boss.GetComponent<Boss>().GetHealth());
                    gameObject.GetComponent<CanvasGroup>().alpha = 1;
                    isBossBorn = true;
                }
            }
        }
        else
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0;
        }
        if (boss == null && isBossBorn)
        {
            SetHealth(0);
            StartCoroutine(hideBarAfterBossDie());
        }
        else if(isBossBorn && boss!=null)
        {
            SetHealth(boss.GetComponent<Boss>().GetHealth());
        }
        if (boss)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
        }
        //for test
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameController.DamagePlayer(1);
        }
        */
    }

    private IEnumerator hideBarAfterBossDie()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }
}
