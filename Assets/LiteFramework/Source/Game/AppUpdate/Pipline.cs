using System.Collections.Generic;
using Lite.Framework.AppUpdate.Stage;

namespace Lite.Framework.AppUpdate
{
    public class Pipline
    {
        private Dictionary<StageID, IStage> Stages_ = new Dictionary<StageID, IStage>();
        private IStage PreStage_ = null;
        private IStage CurStage_ = null;
        private IStage DstStage_ = null;
        private IStage DefaultStage_ = null;

        public Pipline()
        {
        }

        public void InitStage(Updater Updater)
        {
            AddStage(new IdleStage(Updater), StageID.Idle);
            AddStage(new FinalStage(Updater), StageID.Idle);
            AddStage(new ErrorStage(Updater), StageID.Error);
            AddStage(new GetResVersionStage(Updater), StageID.DownloadResInfo);
            AddStage(new DownloadResInfoStage(Updater), StageID.CheckResVersion);
            AddStage(new CheckResVersionStage(Updater), StageID.DownloadRes);
            AddStage(new DownloadResStage(Updater), StageID.CopyResInfoToDoc);
            AddStage(new CopyResInfoToDocStage(Updater), StageID.CheckResVersion);
            
            SetDefaultStage(StageID.GetResVersion);
        }

        public void ClearStage()
        {
            Stages_.Clear();
        }

        public void AddStage(IStage Stage, StageID NextStageID)
        {
            if (!Stages_.ContainsKey(Stage.GetStageID()))
            {
                Stages_.Add(Stage.GetStageID(), Stage);
                Stage.SetNextStageID(NextStageID);
            }
        }
        
        public void Update()
        {
            if (Stages_.Count == 0)
            {
                return;
            }

            if (DstStage_ != null)
            {
                if (CurStage_ != null)
                {
                    CurStage_.Exit();
                }

                PreStage_ = CurStage_;
                CurStage_ = DstStage_;
                DstStage_ = null;

                CurStage_.Enter();
                CurStage_.Update();
            }
            else if (CurStage_ != null)
            {
                CurStage_.Update();
            }
            else
            {
                if (DefaultStage_ != null)
                {
                    CurStage_ = DefaultStage_;
                    CurStage_.Enter();
                    CurStage_.Update();
                }
            }

            if (CurStage_ != null)
            {
                StageCode Code = CurStage_.CheckStage();
                if (Code == StageCode.Failed)
                {
                    TranslateTo(StageID.Error);
                }
                else if (Code == StageCode.Succeeded)
                {
                    TranslateTo(CurStage_.GetNextStageID());
                }
            }
        }

        private void TranslateTo(StageID NextID)
        {
            if (Stages_.ContainsKey(NextID))
            {
                DstStage_ = Stages_[NextID];
            }
            else
            {
                TranslateTo(StageID.Error);
            }
        }

        private void SetDefaultStage(StageID DefaultID)
        {
            if (Stages_.ContainsKey(DefaultID))
            {
                DefaultStage_ = Stages_[DefaultID];
            }
        }
    }
}
