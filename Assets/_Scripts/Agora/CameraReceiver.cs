using UnityEngine;
using Unity.WebRTC;
using Fusion;
using System.Collections;

public class PassthroughWebRTCReceiver : NetworkBehaviour
{
    public RenderTexture receiveTexture;

    private RTCPeerConnection peer;
    private VideoStreamTrack remoteTrack;
    private Coroutine _webRtcUpdateCoroutine;
    
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
        {
            StartCoroutine(StartReceiver());
        }
    }
    private void Awake()
    {
        _webRtcUpdateCoroutine = StartCoroutine(WebRTC.Update());
    }
    IEnumerator StartReceiver()
    {

        peer = new RTCPeerConnection();

        peer.OnTrack = e =>
        {
            if (e.Track is VideoStreamTrack video)
            {
                remoteTrack = video;
                remoteTrack.OnVideoReceived += OnFrame;
            }
        };

        peer.OnIceCandidate = candidate =>
        {
            if (candidate != null)
                RPC_SendIce(candidate.Candidate, candidate.SdpMid, candidate.SdpMLineIndex ?? 0);
        };

        yield return null;
    }

    void OnFrame(Texture tex)
    {
        Graphics.Blit(tex, receiveTexture);
    }

    public void ReceiveOffer(string sdp)
    {
        StartCoroutine(HandleOffer(sdp));
    }

    IEnumerator HandleOffer(string sdp)
    {
        var desc = new RTCSessionDescription
        {
            type = RTCSdpType.Offer,
            sdp = sdp
        };

        var answerOp = peer.CreateAnswer();
        yield return answerOp;

        var answerDesc = answerOp.Desc;

        yield return peer.SetLocalDescription(ref answerDesc);

        RPC_SendAnswer(answerDesc.sdp);

    }

    public void ReceiveIce(string candidate, string mid, int index)
    {
        RTCIceCandidateInit init = new RTCIceCandidateInit
        {
            candidate = candidate,
            sdpMid = mid,
            sdpMLineIndex = index
        };

        peer.AddIceCandidate(new RTCIceCandidate(init));
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SendAnswer(string sdp)
    {
        FindObjectOfType<PassthroughWebRTCSender>().ReceiveAnswer(sdp);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_SendIce(string candidate, string mid, int lineIndex)
    {
        FindObjectOfType<PassthroughWebRTCSender>()
            .ReceiveIce(candidate, mid, lineIndex);
    }
    
    private void OnDestroy()
    {
        if (_webRtcUpdateCoroutine != null)
        {
            StopCoroutine(_webRtcUpdateCoroutine);
            _webRtcUpdateCoroutine = null;
        }
    }
}