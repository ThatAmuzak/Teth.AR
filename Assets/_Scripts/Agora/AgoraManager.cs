using System.Collections;
using _Scripts.Utils;
using Agora.Rtc;
using UnityEngine;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class AgoraManager : MonoBehaviour
{
    private string _appID= "";

    private string token =
        "007eJxTYDjQO/Orh6lfwDcj1s/P2Z6c8O2ZxRPCZ7NWqOui2JzOFCkFBgMDM7NUw6QUIJlkYmaUlpScYp6Yam5ubmxqkZaamNptuTazIZCRYYFXPDMjAwSC+KwMjjkFGYkMDAA8EB7y";
    private string channelName = "Alpha";
    
    internal VideoSurface LocalView;
    internal VideoSurface RemoteView;
    internal IRtcEngine RtcEngine;
    
    
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new() { Permission.Camera, Permission.Microphone };
#endif

    
    private void Awake()
    {
        _appID = SecretsLoader.Secrets["agora"];
    }

    void Start()
    {
        SetupVideoSDKEngine();
        InitEventHandler();
        SetupUI();
        PreviewSelf();
        Join();
    }
    

    private void SetupVideoSDKEngine() 
    {
        // Create an IRtcEngine instance
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        RtcEngineContext context = new();
        context.appId = _appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        // Initialize the instance
        RtcEngine.Initialize(context);
    }
    
    private void InitEventHandler()
    {
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }
    
    public void Join() 
    {
        // Set channel media options
        ChannelMediaOptions options = new ChannelMediaOptions();
        // Start video rendering
        LocalView.SetEnable(true);
        // Automatically subscribe to all audio streams
        options.autoSubscribeAudio.SetValue(true);
        // Automatically subscribe to all video streams
        options.autoSubscribeVideo.SetValue(true);
        // Set the channel profile to live broadcast
        options.channelProfile.SetValue(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        //Set the user role as host
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        // Set the audience latency level
        options.audienceLatencyLevel.SetValue(AUDIENCE_LATENCY_LEVEL_TYPE.AUDIENCE_LATENCY_LEVEL_ULTRA_LOW_LATENCY);
        // Join a channel
        RtcEngine.JoinChannel(token, channelName, 0, options);
    }
   
    private void SetupUI()
    {
        GameObject go = GameObject.Find("LocalView");
        LocalView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        go = GameObject.Find("RemoteView");
        RemoteView = go.AddComponent<VideoSurface>();
        // go.transform.Rotate(0.0f, 0.0f, -180.0f);
        // go = GameObject.Find("Leave");
        // go.GetComponent<Button>().onClick.AddListener(Leave);
        // go = GameObject.Find("Join");
        // go.GetComponent<Button>().onClick.AddListener(Join);
    }
  
    private void PreviewSelf()
    {
        // Enable video module
        RtcEngine.EnableVideo();
        // Start local video preview
        RtcEngine.StartPreview();
        // Set local video display
        LocalView.SetForUser(0, "");
        // Start rendering video
        LocalView.SetEnable(true);
    }
    
    public void Leave()
    {
        Debug.Log("Leaving _channelName");
        // Disable video module
        RtcEngine.StopPreview();
        // Leave the channel
        RtcEngine.LeaveChannel();
        // Stop remote video rendering
        RemoteView.SetEnable(false);
    }
    
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraManager _videoSample;

        internal UserEventHandler(AgoraManager videoSample)
        {
            _videoSample = videoSample;
        }

        // Callback triggered when an error occurs
        public override void OnError(int err, string msg)
        {
        }

        // Callback triggered when the local user successfully joins the channel
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log($"Joined channel {connection.channelId} successfully");
        }

        // OnUserJoined callback is triggered when the SDK receives and successfully decodes the first frame of remote video
        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            // Set remote video display
            _videoSample.RemoteView.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            // Start video rendering
            _videoSample.RemoteView.SetEnable(true);
            Debug.Log("Remote user joined");
        }

        // Callback triggered when a remote user leaves the current channel
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            _videoSample.RemoteView.SetEnable(false);
            Debug.Log("Remote user offline");
        }
    }
}