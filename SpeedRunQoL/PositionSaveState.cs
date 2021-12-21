using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using static DebugMod.EnemiesPanel;
using System;
using DebugMod;
using Console = DebugMod.Console;

namespace SpeedRunQoL
{
    public class PositionSaveState
    {

        public static List<EnemyData> FSMs = new List<EnemyData>();
        public static List<EnemyData> FSMs2 = new List<EnemyData>();

        public static Vector3 KnightPos;
        public static Vector3 CamPos;
        public static Vector2 KnightVel;

        public static void SavePosition()
        {
            KnightPos = HeroController.instance.gameObject.transform.position;
            KnightVel = HeroController.instance.current_velocity;
            CamPos = GameManager.instance.cameraCtrl.gameObject.transform.position;
            
            FSMs = GetAllEnemies(FSMs2);
            FSMs.ForEach(delegate(EnemyData dat) { dat.gameObject.SetActive(false); });
            LoadPosition();

        }

        public static void LoadPosition()
        {
            try
            {
                if (FSMs.Count == 0)
                {
                    Console.AddLine("Cannot load save state because no state is saved");
                    return;
                }
                RemoveAll();
                FSMs2 = Create();
                // Move knight to saved location, change velocity to saved velocity, Move Camera to saved campos, 
                
                HeroController.instance.gameObject.transform.position = KnightPos;
                HeroController.instance.current_velocity = KnightVel;
                GameManager.instance.cameraCtrl.gameObject.transform.position = CamPos;
            }
            catch (Exception e)
            {
                SpeedRunQoL.instance.Log(e.Message);
                SpeedRunQoL.instance.Log(e.StackTrace);
            }
        }

        public static List<EnemyData> Create()
        {
            List<EnemyData> data = new List<EnemyData>();
            
            for (int i = 0; i < FSMs.Count; i++)
            {
                Console.AddLine(i.ToString());
                EnemyData dattemp = FSMs.FindAll(ed => ed.gameObject != null)[i];

                GameObject gameObject2 = UnityEngine.Object.Instantiate(dattemp.gameObject,
                    dattemp.gameObject.transform.position, dattemp.gameObject.transform.rotation) as GameObject;
                Component component = gameObject2.GetComponent<tk2dSprite>();

                HealthManager hm = gameObject2.GetComponent<HealthManager>();
                int value8 = hm.hp;
                gameObject2.SetActive(true);
                data.Add(new EnemyData(value8, hm, component, DebugMod.EnemiesPanel.parent, gameObject2));
            }

            ;
            return data;
        }

        public static void RemoveAll()
        {
            //get all created
            FSMs2.ForEach(delegate(EnemyData dat)
            {
                if (!FSMs.Any(ed => ed.gameObject == dat.gameObject))
                    GameObject.Destroy(dat.gameObject.gameObject.gameObject.gameObject);

            });

        }

        public static List<EnemyData> GetAllEnemies(List<EnemyData> Exclude)
        {
            float boxSize = 250f;
            List<EnemyData> ret = new List<EnemyData>();
            if (HeroController.instance != null && !HeroController.instance.cState.transitioning && GameManager.instance.IsGameplayScene())
            {
                int layerMask = 133120;
                Collider2D[] array = Physics2D.OverlapBoxAll(HeroController.instance.transform.position, new Vector2(boxSize, boxSize), 1f, layerMask);
                if (array != null)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        HealthManager hm = array[i].GetComponent<HealthManager>();

                        if (hm && array[i].gameObject.activeSelf &&
                            //!(Exclude.Any(ed => ed.gameObject == array[i].gameObject)) &&
                            !Ignore(array[i].gameObject.name))
                        {
                            Component component = array[i].gameObject.GetComponent<tk2dSprite>();
                            if (component == null)
                            {
                                component = null;
                            }

                            int value = hm.hp;
                            ret.Add(new EnemyData(value, hm, component, DebugMod.EnemiesPanel.parent, array[i].gameObject));
                        }
                    }
                }
            }

            return ret;
        }
    }
}
