using System;
using LiteFramework.Helper;
using UnityEngine;
using UnityEngine.UI;

namespace LiteFramework.AppUpdate
{
    public class UpdateUI
    {
        private StageProgressID ProgressID_;
        private Updater Updater_;
        private UpdateTextInfo TextInfo_;
        private GameObject UIRoot_;
        private Scrollbar ProgressBar_;
        private Text LabelProgress_;
        private Text LabelContent_;

        public event Action<bool> Completed; 

        public UpdateUI()
        {
            ProgressID_ = StageProgressID.Idle;
            TextInfo_ = UpdateTool.GetUpdateTextInfo();
            
            UIRoot_ = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("UpdateUI"));
            UIRoot_.transform.SetParent(GameObject.Find("Canvas-Normal").transform);
            RectTransform Rect = UIRoot_.GetComponent<RectTransform>();
            Rect.localPosition = Vector3.zero;
            Rect.localRotation = Quaternion.identity;
            Rect.localScale = Vector3.one;
            Rect.sizeDelta = Vector2.zero;

            ProgressBar_ = UIHelper.GetComponent<Scrollbar>(UIRoot_.transform, "ProgressBar");
            LabelProgress_ = UIHelper.GetComponent<Text>(UIRoot_.transform, "LabelProgress");
            LabelContent_ = UIHelper.GetComponent<Text>(UIRoot_.transform, "LabelContent");

            ProgressBar_.value = 0;
            LabelProgress_.text = string.Empty;
            LabelContent_.text = string.Empty;

            Updater_ = new Updater();

        }

        public void Stop()
        {
            if (Updater_ != null)
            {
                Updater_.Stop();
            }
        }

        public void Update(float DeltaTime)
        {
            /*if (!ServerConfig.EnableAppUpdater_)
            {
                if (Completed != null)
                {
                    Completed(true);
                    UnityEngine.Object.Destroy(UIRoot_);
                }

                return;
            }*/

            Updater_.Update(DeltaTime);

            ErrorCode Code = Updater_.GetErrorCode();

            if (Code != ErrorCode.Ok)
            {
                LabelContent_.text = TextInfo_[Code.ToString()];
            }
            else
            {
                LabelContent_.text = Updater_.GetContentInfo();
            }

            StageProgressID ProgressID = Updater_.GetProgressID();

            if (ProgressID != ProgressID_)
            {
                ProgressID_ = ProgressID;
                LabelProgress_.text = TextInfo_[ProgressID_.ToString()];
            }

            float Value = Updater_.GetProgressValue();
            ProgressBar_.size = Value;

            if (Updater_.IsDone())
            {
                if (Completed != null)
                {
                    Completed(Code == ErrorCode.Ok);
                    UnityEngine.Object.Destroy(UIRoot_);
                }
            }
        }
    }
}
