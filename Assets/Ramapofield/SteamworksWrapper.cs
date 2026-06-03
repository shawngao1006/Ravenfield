using System;
using System.Collections.Generic;
using UnityEngine;

// Minimal no-op Steamworks replacements for editor builds without STEAMWORKS.
namespace Steamworks
{
    public struct PublishedFileId_t
    {
        public ulong m_PublishedFileId;
        public PublishedFileId_t(ulong v) { m_PublishedFileId = v; }
        public override string ToString() { return m_PublishedFileId.ToString(); }
    }

    public struct SteamUGCDetails_t { public PublishedFileId_t m_nPublishedFileId; }
    public struct UGCQueryHandle_t { public ulong handle; public UGCQueryHandle_t(ulong v) { handle = v; } }
    public struct UGCUpdateHandle_t { public ulong handle; public UGCUpdateHandle_t(ulong v) { handle = v; } }
    public struct SteamAPICall_t { public ulong m_Handle; public SteamAPICall_t(ulong v) { m_Handle = v; } }
    public struct CSteamID { public ulong m_SteamID; public CSteamID(ulong id) { m_SteamID = id; } }
    public struct AppId_t { public uint id; public AppId_t(uint v) { id = v; } }
    public enum EResult { k_EResultOK = 1 }
    public enum EItemUpdateStatus { k_EItemUpdateStatusPreparingContent, k_EItemUpdateStatusPreparingConfig }
    public enum ERemoteStoragePublishedFileVisibility { k_ERemoteStoragePublishedFileVisibilityPrivate, k_ERemoteStoragePublishedFileVisibilityFriendsOnly, k_ERemoteStoragePublishedFileVisibilityPublic }
    public enum EUGCQuery { k_EUGCQuery_RankedByVotesUp }
    public enum EUGCMatchingUGCType { k_EUGCMatchingUGCType_Items_ReadyToUse }

    public struct InputAnalogActionHandle_t { public ulong handle; public InputAnalogActionHandle_t(ulong v) { handle = v; } }
    public struct InputDigitalActionHandle_t { public ulong handle; public InputDigitalActionHandle_t(ulong v) { handle = v; } }
    public struct InputAnalogActionData_t { }
}

public class SteamworksWrapper
{
    public const uint APP_ID = 636480U;
    public const string WORKSHOP_TERMS_OF_SERVICE_URL = "http://steamcommunity.com/sharedfiles/workshoplegalagreement";
    public const string COMMUNITY_FILE_PAGE_URL = "steam://url/CommunityFilePage/";

    public SteamInputWrapper input = new SteamInputWrapper();
    public bool isInitialized = false;
    public Steamworks.EResult lastResult = 0;
    public string errorMessage = null;
    public Steamworks.PublishedFileId_t[] subscribedItems = null;
    public SteamworksWrapper.WorkshopItem currentItem = null;

    public delegate void DelOnStateChanged();
    public delegate void DelOnCreateItemDone(bool ok, Steamworks.PublishedFileId_t itemId);
    public delegate void DelOnSubmitItemDone(bool ok);
    public delegate void DelOnUGCQueryDone(bool ok, SteamworksWrapper.UGCQueryResult[] details);
    public delegate void DelOnItemInstalled(Steamworks.PublishedFileId_t itemId);
    public delegate void DelOnRemotePublishedFileSubscribed(Steamworks.PublishedFileId_t itemId);
    public delegate void DelOnRemotePublishedFileUnsubscribed(Steamworks.PublishedFileId_t itemId);

    public DelOnStateChanged OnStateChanged;
    public DelOnCreateItemDone OnCreateItemDone;
    public DelOnSubmitItemDone OnSubmitItemDone;
    public DelOnItemInstalled OnItemInstalled;
    public DelOnRemotePublishedFileSubscribed OnRemotePublishedFileSubscribed;
    public DelOnRemotePublishedFileUnsubscribed OnRemotePublishedFileUnsubscribed;

    public SteamworksWrapper() { }

    private void SignalStateChanged() { if (OnStateChanged != null) OnStateChanged(); }

    public bool Initialize() { isInitialized = false; SignalStateChanged(); return false; }
    public void Update() { }
    public void Shutdown() { isInitialized = false; SignalStateChanged(); }
    public string Username() { return "LocalUser"; }
    public bool HasCurrentItem() { return currentItem != null; }
    public void SetCurrentItem(SteamworksWrapper.WorkshopItem item) { currentItem = item; SignalStateChanged(); }
    public void SetCurrentItemId(ulong id) { SetCurrentItem(new WorkshopItem(new Steamworks.PublishedFileId_t(id))); }
    public void DropCurrentItem() { currentItem = null; }

    public void CreateWorkshopItem() { }
    public void SubmitCurrentItem(string contentPath, string changeNote) { }
    public bool IsUploadingItem() { return false; }
    public bool IsPreparingContentUpload() { return false; }
    public float GetUploadProgress() { return 0f; }

    public Steamworks.UGCQueryHandle_t CreateUGCQuery(DelOnUGCQueryDone ManagedCallback) { return new Steamworks.UGCQueryHandle_t(0UL); }
    public void AddRequiredTagUGCQuery(Steamworks.UGCQueryHandle_t query, string tag) { }
    public void SendUGCQueryRequest(Steamworks.UGCQueryHandle_t query) { }
    public void QuickQueryItemInfo(Steamworks.PublishedFileId_t[] fileIds, DelOnUGCQueryDone ManagedCallback) { ManagedCallback(true, new UGCQueryResult[0]); }

    public void SubscribeToItem(Steamworks.PublishedFileId_t fileId) { }
    public uint GetItemState(Steamworks.PublishedFileId_t fileId) { return 0U; }
    public uint FetchSubscribedItems() { subscribedItems = new Steamworks.PublishedFileId_t[0]; return 0U; }
    public bool HasFetchedSubscribedItems() { return subscribedItems != null; }
    public bool IsSubscribedItemInstalled(Steamworks.PublishedFileId_t itemId) { return false; }
    public string[] GetSubscribedItemPaths() { return new string[0]; }
    public string GetSubscribedItemPath(Steamworks.PublishedFileId_t id) { return string.Empty; }

    public string GetSteamNick() { return "LocalUser"; }
    public ulong GetSteamId() { return 0UL; }

    private void SetError(string error) { this.errorMessage = error; }

    public void OpenCommunityFilePage(Steamworks.PublishedFileId_t itemId) { OpenUrl(COMMUNITY_FILE_PAGE_URL + itemId.m_PublishedFileId.ToString(), false); }
    public void OpenUrl(string url, bool inSteamOverlay = true) { Application.OpenURL(url); }

    public class WorkshopItem
    {
        public Steamworks.PublishedFileId_t itemId;
        public string title;
        public string description;
        public string previewImagePath;
        public List<string> tags;
        public WorkshopItem.Visibility visibility;
        public WorkshopItem(Steamworks.PublishedFileId_t itemId) { this.itemId = itemId; this.visibility = WorkshopItem.Visibility.NoChange; }
        public override string ToString() { return "WorkshopItem #" + itemId.m_PublishedFileId.ToString(); }
        public enum Visibility { NoChange, Private, FriendsOnly, Public }
    }

    public struct UGCQueryResult { public Steamworks.SteamUGCDetails_t details; public string previewImageURL; }
}
