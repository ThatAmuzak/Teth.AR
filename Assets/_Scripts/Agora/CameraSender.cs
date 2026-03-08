using UnityEngine;
using Unity.WebRTC;
using Fusion;
using System.Collections;
using Meta.XR;

public class PassthroughWebRTCSender : NetworkBehaviour
{
    public RenderTexture sendTexture;
    public Material blitMat;
    [SerializeField] private PassthroughCameraAccess passthroughCamera;
    private RTCPeerConnection peer;
    private VideoStreamTrack videoTrack;
    
    Texture cameraTex;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            StartCoroutine(StartSender());
        }
    }

    IEnumerator StartSender()
    {

        peer = new RTCPeerConnection();

        videoTrack = new VideoStreamTrack(sendTexture);
        peer.AddTrack(videoTrack);

        peer.OnIceCandidate = candidate =>
        {
            if (candidate != null)
                RPC_SendIce(candidate.Candidate, candidate.SdpMid, candidate.SdpMLineIndex ?? 0);
        };
        var offerOp = peer.CreateOffer();
        yield return offerOp;

        var offerDesc = offerOp.Desc;

        yield return peer.SetLocalDescription(ref offerDesc);

        RPC_SendOffer(offerDesc.sdp);
    }

    void Update()
    {
        if (!Object.HasStateAuthority) return;

        cameraTex =  passthroughCamera.GetTexture();

        if (cameraTex != null)
        {
            Graphics.Blit(cameraTex, sendTexture, blitMat);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SendOffer(string sdp)
    {
        FindObjectOfType<PassthroughWebRTCReceiver>().ReceiveOffer(sdp);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SendIce(string candidate, string mid, int lineIndex)
    {
        FindObjectOfType<PassthroughWebRTCReceiver>()
            .ReceiveIce(candidate, mid, lineIndex);
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
    public void ReceiveAnswer(string sdp)
    {
        var desc = new RTCSessionDescription
        {
            type = RTCSdpType.Answer,
            sdp = sdp
        };

        peer.SetRemoteDescription(ref desc);
    }
    
}