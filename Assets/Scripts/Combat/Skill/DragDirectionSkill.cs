using LiteMore.Helper;
using UnityEngine;

namespace LiteMore.Combat.Skill
{
    public class DragDirectionSkill : DragSkill
    {
        public event System.Action<SkillDescriptor, Vector2> OnUsed;

        private readonly Vector2 OriginPos_;
        private Transform DragObj_;

        public DragDirectionSkill(Transform Trans, SkillDescriptor Desc, Vector2 OriginPos)
            : base(Trans, Desc)
        {
            OriginPos_ = OriginPos;

            DragObj_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/bv1")).transform;
            DragObj_.SetParent(Configure.SfxRoot, false);
            DragObj_.localPosition = OriginPos_;
            DragObj_.gameObject.SetActive(false);
            var SR = DragObj_.GetComponent<SpriteRenderer>();
            SR.color = Color.green;
            SR.size = new Vector2(256, 256);
        }

        public override void Destroy()
        {
            if (DragObj_ != null)
            {
                Object.Destroy(DragObj_.gameObject);
                DragObj_ = null;
            }

            base.Destroy();
        }

        private void CreateDragObj()
        {
            if (DragObj_ != null)
            {
                return;
            }

            DragObj_ = Object.Instantiate(Resources.Load<GameObject>("Prefabs/bv0")).transform;
            DragObj_.SetParent(Configure.SfxRoot, false);
            DragObj_.localPosition = Vector3.zero;
            DragObj_.gameObject.SetActive(false);
            var SR = DragObj_.GetComponent<SpriteRenderer>();
            SR.color = Color.green;
            SR.size = new Vector2(Desc_.Radius * 2, Desc_.Radius * 2);
        }

        protected override void OnBeginDrag(Vector2 Pos)
        {
            CreateDragObj();
            DragObj_.gameObject.SetActive(true);
        }

        protected override void OnDrag(Vector2 Pos)
        {
            if (Vector2.Distance(Pos, Transform_.position) < IconTransform_.sizeDelta.y / 2)
            {
                DragObj_.gameObject.SetActive(false);
            }
            else
            {
                DragObj_.gameObject.SetActive(true);
                var Angle = MathHelper.GetUnityAngle(OriginPos_, Pos);
                DragObj_.localRotation = Quaternion.AngleAxis(Angle, Vector3.forward);
            }
        }

        protected override void OnEndDrag(Vector2 Pos)
        {
            if (!DragObj_.gameObject.activeSelf)
            {
                return;
            }

            DragObj_.gameObject.SetActive(false);
            OnDragSpell(Pos);
        }

        private void OnDragSpell(Vector2 Pos)
        {
            if (IsCD_)
            {
                return;
            }

            if (PlayerManager.Mp < Desc_.Cost)
            {
                return;
            }

            StartCD();
            PlayerManager.AddMp(-Desc_.Cost);
            OnUsed?.Invoke(Desc_, (Pos - OriginPos_).normalized);
        }
    }
}