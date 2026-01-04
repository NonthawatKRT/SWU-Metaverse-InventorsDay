using System;
using System.Collections.Generic;
using PurrNet.Logging;
using UnityEngine;

namespace PurrLobby
{
    public class LobbyMemberList : MonoBehaviour
    {
        [SerializeField] private MemberEntry memberEntryPrefab;
        [SerializeField] private Transform content;

        private void Start()
        {
            EnsureContentAssigned();
        }
        private bool IsContentValid()
        {
            return content != null && content.gameObject != null;
        }


        public void EnsureContentAssigned()
        {
            if (content != null && content.gameObject != null)
                return;

            GameObject memberContentObj = GameObject.Find("MemberContent");
            if (memberContentObj != null)
            {
                content = memberContentObj.transform;
            }
        }


        public void LobbyDataUpdate(Lobby room)
        {
            EnsureContentAssigned();
            if (!room.IsValid || !IsContentValid())
                return;

            HandleExistingMembers(room);
            HandleNewMembers(room);
            HandleLeftMembers(room);
        }


        public void OnLobbyLeave()
        {
            EnsureContentAssigned();
            if (!IsContentValid())
                return;

            foreach (Transform child in content)
                Destroy(child.gameObject);
        }

        private void HandleExistingMembers(Lobby room)
        {
            if (!IsContentValid())
                return;

            foreach (Transform child in content)
            {
                if (!child || !child.TryGetComponent(out MemberEntry member))
                    continue;

                var matchingMember = room.Members.Find(x => x.Id == member.MemberId);
                if (!string.IsNullOrEmpty(matchingMember.Id))
                {
                    member.SetReady(matchingMember.IsReady);
                }
            }
        }


        private void HandleNewMembers(Lobby room)
        {
            if (!IsContentValid())
                return;

            var existingMembers = content.GetComponentsInChildren<MemberEntry>();

            foreach (var member in room.Members)
            {
                if (Array.Exists(existingMembers, x => x.MemberId == member.Id))
                    continue;

                var entry = Instantiate(memberEntryPrefab, content);
                entry.Init(member);
            }
        }


        private void HandleLeftMembers(Lobby room)
        {
            if (!IsContentValid())
                return;

            var childrenToRemove = new List<Transform>();

            for (int i = 0; i < content.childCount; i++)
            {
                var child = content.GetChild(i);
                if (!child || !child.TryGetComponent(out MemberEntry member))
                    continue;

                if (!room.Members.Exists(x => x.Id == member.MemberId))
                {
                    childrenToRemove.Add(child);
                }
            }

            foreach (var child in childrenToRemove)
            {
                if (child)
                    Destroy(child.gameObject);
            }
        }

        private void OnDestroy()
        {
            var lobby = FindObjectOfType<LobbyManager>();
            if (lobby != null)
            {
                lobby.OnRoomUpdated.RemoveListener(LobbyDataUpdate);
            }
        }


    }
}
