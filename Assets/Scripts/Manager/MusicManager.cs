using System.Collections;
using Assets.Scripts.Tool;
using UnityEngine;
using Assets.Scripts.Config;
namespace Assets.Scripts.Manager
{
    //音乐管理类，继承单例类
    public class MusicManager : MonoSingleton<MusicManager>
    {
        // 用于播放背景音乐的音乐源
        private AudioSource s_bgMusic;

        // 用于播放音效的音乐源
        private AudioSource s_effectMusic;

        // 控制背景音乐音量大小
        public float BgVolume
        {
            get
            {
                return s_bgMusic.volume;
            }
            set
            {
                s_bgMusic.volume = value;
            }
        }
        //控制音效音量的大小
        public float EffectVolmue
        {
            get
            {
                return s_effectMusic.volume;
            }
            set
            {
                s_effectMusic.volume = value;
            }
        }

        //重写虚方法
        protected override void Awake()
        {
            base.Awake();
            //实例化音乐源
            s_bgMusic = gameObject.AddComponent<AudioSource>();
            s_bgMusic.loop = true;    //开启循环
            s_bgMusic.playOnAwake = false;        //开始播放

            //实例化音乐源
            s_effectMusic = gameObject.AddComponent<AudioSource>();
            s_effectMusic.loop = true;
            s_effectMusic.playOnAwake = false;
        }

        // 播放背景音乐，传进一个音频剪辑的name
        private void PlayBgBase(object bgName, bool restart = false)
        {
            //定义一个空的字符串
            string curBgName = string.Empty;
            //如果这个音乐源的音频剪辑不为空的话
            if (s_bgMusic.clip != null)
            {
                //得到这个音频剪辑的name
                curBgName = s_bgMusic.clip.name;
            }

            // 根据用户的音频片段名称, 找到AuioClip, 然后播放,
            //ResManager是提前定义好的查找音频剪辑对应路径的单例脚本，并动态加载出来
            AudioClip clip = ResManager.Instance.Load<AudioClip>(bgName);
            //如果找到了，不为空
            if (clip != null)
            {
                //如果这个音频剪辑已经复制给类音频源，切正在播放，那么直接跳出
                if (clip.name == curBgName && !restart)
                {
                    return;
                }
                //否则，把改音频剪辑赋值给音频源，然后播放
                s_bgMusic.clip = clip;
                s_bgMusic.Play();
            }
            else
            {
                //没找到直接报错
                // 异常, 调用写日志的工具类.
                UnityEngine.Debug.Log("没有找到音频片段");
            }
        }

        //播放各种音频剪辑的调用方法，MusicType是提前写好的存放各种音乐名称的枚举类，便于外面直接调用
        public void PlayBg(MusicType.Music bgName, bool restart = false)
        {
            PlayBgBase(bgName, restart);
        }

        // 播放音效
        private void PlayEffectBase(object effectName, bool defAudio = true, float volume = 1f)
        {
            //根据查找路径加载对应的音频剪辑
            AudioClip clip = ResManager.Instance.Load<AudioClip>(effectName);
            //如果为空的话，直接报错，然后跳出
            if (clip == null)
            {
                UnityEngine.Debug.Log("没有找到音效片段");
                return;
            }
            //否则，就是clip不为空的话，如果defAudio=true，直接播放
            if (defAudio)
            {
                //PlayOneShot (音频剪辑, 音量大小)
                s_effectMusic.PlayOneShot(clip, volume);
            }
            else
            {
                //指定点播放
                AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
            }
        }

        //播放各种音频剪辑的调用方法，MusicType是提前写好的存放各种音乐名称的枚举类，便于外面直接调用
        public void PlayEffect(MusicType.Music effectName, bool defAudio = true, float volume = 1f)
        {
            PlayEffectBase(effectName, defAudio, volume);
        }
    }
}
