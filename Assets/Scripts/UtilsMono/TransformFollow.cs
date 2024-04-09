using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 跟随目标Transform的工具
/// </summary>
public class TransformFollow : MonoBehaviour
{
    [FieldName("启用延时")]
    public float EnableDelay;
    private float m_CurEnableDelay;
    [FieldName("不停止")]
    public bool DontStop;
    //[FieldName("跟随目标枚举")]
    //public EnumTargetId TargetEnum;
    [Tooltip("跟随的目标")]
    public Transform Target;
    [Tooltip("位置旋转缩放都跟随，世界坐标模式")]
    public bool FullyFollowWorld;
    [Tooltip("位置旋转缩放都跟随，局部坐标模式")]
    public bool FullyFollowLocal;
    [Tooltip("位置旋转缩放都跟随，位置跟随的是AnchoredPosition，旋转缩放是世界坐标模式")]
    public bool FullyFollowAnchored;    
    [Tooltip("位置跟随的设置")]
    public Setting1 Position;
    [Tooltip("旋转跟随的设置")]
    public Setting2 Rotation;
    [Tooltip("缩放跟随的设置")]
    public Setting3 Scale;
    [System.Serializable]
    public struct Setting1
    {
        [Tooltip("X轴是否跟随")]
        public bool X;
        [Tooltip("Y轴是否跟随")]
        public bool Y;
        [Tooltip("Z轴是否跟随")]
        public bool Z;
        [Tooltip("是否是局部坐标模式的跟随")]
        public bool Local;
        [Tooltip("跟随的平滑度，为0时没有平滑过度")]
        public float Smooth;
        [FieldName("速度，匀速模式用")]
        public float Speed;

        [FieldName("最大速度")]
        public float MaxSpeed;//最大速度
        [FieldName("减速半径")]
        public float SpeedDownRadius;//减速半径 
        [FieldName("加速度")]
        public float AcceSpeed;//加速度

        [Tooltip("位置跟随是否是跟随AnchoredPosition")]
        public bool IsAnchoredPosition;
        [Tooltip("跟随的偏移值")]
        public Vector3 Offset;
       
    }
    [System.Serializable]
    public struct Setting2
    {
        [Tooltip("X轴是否跟随")]
        public bool X;
        [Tooltip("Y轴是否跟随")]
        public bool Y;
        [Tooltip("Z轴是否跟随")]
        public bool Z;
        [Tooltip("是否是局部旋转模式的跟随")]
        public bool Local;
        [Tooltip("跟随的平滑度，为0时没有平滑过度")]
        public float Smooth;
        [FieldName("速度，匀速模式用")]
        public float Speed;
        [Tooltip("跟随的偏移值")]
        public Vector3 Offset;
    }
    [System.Serializable]
    public struct Setting3
    {
        [Tooltip("X轴是否跟随")]
        public bool X;
        [Tooltip("Y轴是否跟随")]
        public bool Y;
        [Tooltip("Z轴是否跟随")]
        public bool Z;
        [Tooltip("跟随的平滑度，为0时没有平滑过度")]
        public float Smooth;
        [FieldName("速度，匀速模式用")]
        public float Speed;
        [Tooltip("跟随的偏移值")]     
        public Vector3 Offset;
    }   
    public bool Enable=true;
    //自身的RectTransform的引用
    private RectTransform RectTransformSelf;
    //目标的RectTransform的引用
    private RectTransform RectTransformTarget;
    public bool FixedMode = true;

    public bool FaZhenZhuanYong;

    private void OnEnable()
    {
        m_CurEnableDelay = EnableDelay;
    }

    private void Update()
    {
        if (FixedMode) return;
        if (!Enable) return;
        m_CurEnableDelay -= Time.deltaTime;
        if (m_CurEnableDelay > 0) return;

        //如果没有设置目标就直接返回
        if (Target == null)
        {
            //if (TargetEnum != EnumTargetId.None)
            //{
            //    Target = TargetBuffer.TargetTransStack[TargetEnum].Peek();
            //}
            return;
        }
        if (FullyFollowWorld)
        {
            transform.position = Target.position;
            transform.eulerAngles = Target.eulerAngles;
            transform.localScale = Target.localScale;
            return;
        }
        if (FullyFollowLocal)
        {
            transform.localPosition = Target.localPosition;
            transform.localEulerAngles = Target.localEulerAngles;
            transform.localScale = Target.localScale;
            return;
        }
        if (FullyFollowAnchored)
        {
            if (RectTransformSelf == null)
            {
                RectTransformSelf = GetComponent<RectTransform>();
            }
            if (RectTransformTarget == null)
            {
                RectTransformTarget = Target.GetComponent<RectTransform>();
            }
            RectTransformSelf.anchoredPosition = RectTransformTarget.anchoredPosition;
            RectTransformSelf.eulerAngles = RectTransformTarget.eulerAngles;
            RectTransformSelf.localScale = RectTransformTarget.localScale;
            return;
        }

        //位置跟随
        if (Position.IsAnchoredPosition)
        {
            //如果是跟随目标相对于锚点的坐标，就需要先获取一下各自的RectTransform组件
            if (RectTransformSelf == null)
            {
                RectTransformSelf = GetComponent<RectTransform>();
            }
            if (RectTransformTarget == null)
            {
                RectTransformTarget = Target.GetComponent<RectTransform>();
            }
            //跟随勾选了的轴  如果平滑度不设置，则会直接同步坐标，否则就会有平滑过度的效果 
            RectTransformSelf.anchoredPosition = GetValueUpdate(RectTransformSelf.anchoredPosition, RectTransformTarget.anchoredPosition, Position.Offset, Position.Smooth, Position.X, Position.Y, Position.Z,Position.Speed);
        }
        else
        {
            //区分一下 是局部坐标之间的跟随 还是世界坐标之间的跟随
            if (Position.Local)
            {
                transform.localPosition = GetValueUpdate(transform.localPosition, Target.localPosition, Position.Offset, Position.Smooth, Position.X, Position.Y, Position.Z,Position.Speed,Position.MaxSpeed,Position.SpeedDownRadius,Position.AcceSpeed);
            }
            else
            {
                transform.position = GetValueUpdate(transform.position, Target.position, Position.Offset, Position.Smooth, Position.X, Position.Y, Position.Z,Position.Speed, Position.MaxSpeed, Position.SpeedDownRadius, Position.AcceSpeed);
            }
        }

        //旋转跟随
        if (Rotation.Local)
        {
            transform.localEulerAngles = GetValueUpdate(transform.localEulerAngles, Target.localEulerAngles, Rotation.Offset, Rotation.Smooth, Rotation.X, Rotation.Y, Rotation.Z,Rotation.Speed);
        }
        else
        {
            transform.eulerAngles = GetValueUpdate(transform.eulerAngles, Target.eulerAngles, Rotation.Offset, Rotation.Smooth, Rotation.X, Rotation.Y, Rotation.Z,Rotation.Speed);
        }

        //缩放跟随
        transform.localScale = GetValueUpdate(transform.localScale, Target.localScale, Scale.Offset, Scale.Smooth, Scale.X, Scale.Y, Scale.Z,Scale.Speed);
    }
    private void FixedUpdate()
    {
        if (!FixedMode) return;
        if (!Enable) return;
        m_CurEnableDelay -= Time.fixedDeltaTime;
        if (m_CurEnableDelay > 0) return;
        //如果没有设置目标就直接返回
        if (Target == null)
        {
            //if (TargetEnum != EnumTargetId.None)
            //{
            //    Target = TargetBuffer.TargetTransStack[TargetEnum].Peek();
            //}
            return;
        }
        if (FullyFollowWorld)
        {
            transform.position = Target.position;
            transform.eulerAngles = Target.eulerAngles;
            transform.localScale = Target.localScale;
            return;
        }
        if (FullyFollowLocal)
        {
            transform.localPosition = Target.localPosition;
            transform.localEulerAngles = Target.localEulerAngles;
            transform.localScale = Target.localScale;
            return;
        }
        if (FullyFollowAnchored)
        {
            if (RectTransformSelf == null)
            {
                RectTransformSelf = GetComponent<RectTransform>();
            }
            if (RectTransformTarget == null)
            {
                RectTransformTarget = Target.GetComponent<RectTransform>();
            }
            RectTransformSelf.anchoredPosition = RectTransformTarget.anchoredPosition;
            RectTransformSelf.eulerAngles = RectTransformTarget.eulerAngles;
            RectTransformSelf.localScale = RectTransformTarget.localScale;
            return;
        }
      
        //位置跟随
        if (Position.IsAnchoredPosition)
        {
            //如果是跟随目标相对于锚点的坐标，就需要先获取一下各自的RectTransform组件
            if (RectTransformSelf == null)
            {
                RectTransformSelf = GetComponent<RectTransform>();
            }
            if (RectTransformTarget == null)
            {
                RectTransformTarget =Target.GetComponent<RectTransform>();
            }
            //跟随勾选了的轴  如果平滑度不设置，则会直接同步坐标，否则就会有平滑过度的效果 
            RectTransformSelf.anchoredPosition = GetValue(RectTransformSelf.anchoredPosition, RectTransformTarget.anchoredPosition, Position.Offset, Position.Smooth, Position.X, Position.Y, Position.Z,Position.Speed);
        }
        else
        {
            //区分一下 是局部坐标之间的跟随 还是世界坐标之间的跟随
            if (Position.Local)
            {
                transform.localPosition = GetValue(transform.localPosition, Target.localPosition, Position.Offset, Position.Smooth, Position.X, Position.Y, Position.Z,Position.Speed, Position.MaxSpeed, Position.SpeedDownRadius, Position.AcceSpeed);
            }
            else
            {
                transform.position= GetValue(transform.position, Target.position, Position.Offset, Position.Smooth, Position.X, Position.Y, Position.Z,Position.Speed, Position.MaxSpeed, Position.SpeedDownRadius, Position.AcceSpeed);
            }
        }


        //旋转跟随
        if (Rotation.Local)
        {
            transform.localEulerAngles = GetValue(transform.localEulerAngles, Target.localEulerAngles, Rotation.Offset, Rotation.Smooth, Rotation.X, Rotation.Y, Rotation.Z,Rotation.Speed);
        }
        else
        {
            transform.eulerAngles = GetValue(transform.eulerAngles, Target.eulerAngles, Rotation.Offset, Rotation.Smooth,Rotation.X, Rotation.Y, Rotation.Z,Rotation.Speed);
        }

        //缩放跟随
        transform.localScale = GetValue(transform.localScale,Target.localScale,Scale.Offset,Scale.Smooth,Scale.X,Scale.Y,Scale.Z,Scale.Speed);

    }

    private Vector3 GetValue(Vector3 self, Vector3 target, Vector3 offset, float smooth,bool x,bool y,bool z,float speed, float maxSpeed = 0, float speedDownRadius = 0, float acceSpeed = 0)
    {
        offset = new Vector3(offset.x, FaZhenZhuanYong ? offset.y * Player.Instance.transform.localScale.y : offset.y);
        if (speed!=0)
        {
            Vector3 dir = (target + offset) - self;
            if (dir.magnitude < 2 * speed * Time.fixedDeltaTime && !DontStop)
            {
                return self;
            }
            return self + dir.normalized * speed*Time.fixedDeltaTime;
        }
        else if (maxSpeed!=0)
        {
            //判断是否需要减速
            if (Mathf.Abs(self.x - target.x) < speedDownRadius)
            {
                //1.减速 比0大就减速到0  比0小就加速到0
                float delta = (m_CurSpeed > 0 ? -1 : 1) * acceSpeed * Time.fixedDeltaTime;
                m_CurSpeed = Mathf.Clamp(m_CurSpeed + delta, -maxSpeed, maxSpeed);
                //2.如果当前速度还不够小 就设置一下Boss位置 如果速度很小了  这里就不修改Boss位置了
                if (m_CurSpeed > acceSpeed * Time.fixedDeltaTime * 1.5f)
                {
                    return self + new Vector3( m_CurSpeed * Time.fixedDeltaTime,0);
                }
            }
            //加速到最大速度
            else
            {
                //1.计算加速度方向
                int dir = target.x > self.x ? 1 : -1;
                //2.更新速度
                m_CurSpeed = Mathf.Clamp(m_CurSpeed + dir * acceSpeed * Time.fixedDeltaTime, -maxSpeed, maxSpeed);
                //3.计算目标点位
                return self + new Vector3( m_CurSpeed * Time.fixedDeltaTime, 0);
            }
            return self;
        }
        else
        {
            float a = x ? Mathf.Lerp(self.x, target.x + offset.x, smooth == 0 ? 1 : Time.fixedDeltaTime / smooth) : self.x;
            float b = y ? Mathf.Lerp(self.y, target.y + offset.y, smooth == 0 ? 1 : Time.fixedDeltaTime / smooth) : self.y;
            float c = z ? Mathf.Lerp(self.z, target.z + offset.z, smooth == 0 ? 1 : Time.fixedDeltaTime / smooth) : self.z;
            return new Vector3(a, b, c);
        }
      
    }

    private float m_CurSpeed;//当前跟踪速度

    private Vector3 GetValueUpdate(Vector3 self, Vector3 target, Vector3 offset, float smooth, bool x, bool y, bool z,float speed,float maxSpeed=0,float speedDownRadius=0,float acceSpeed=0)
    {
        if (speed!=0)
        {
            Vector3 dir = (target + offset) - self;
            if (dir.magnitude<2* speed * Time.deltaTime&&!DontStop)
            {
                return self;
            }
            return self + dir.normalized * speed * Time.deltaTime;
        }
        else if (maxSpeed!=0)
        {
            //判断是否需要减速
            if (Mathf.Abs(self.x - target.x) < speedDownRadius)
            {
                //1.减速 比0大就减速到0  比0小就加速到0
                float delta = (m_CurSpeed > 0 ? -1 : 1) * acceSpeed * Time.deltaTime;
                m_CurSpeed = Mathf.Clamp(m_CurSpeed + delta, -maxSpeed, maxSpeed);
                //2.如果当前速度还不够小 就设置一下Boss位置 如果速度很小了  这里就不修改Boss位置了
                if (m_CurSpeed > acceSpeed * Time.deltaTime * 1.5f)
                {
                    return self + new Vector3(m_CurSpeed * Time.deltaTime, 0);
                }
            }
            //加速到最大速度
            else
            {
                //1.计算加速度方向
                int dir = target.x > self.x ? 1 : -1;
                //2.更新速度
                m_CurSpeed = Mathf.Clamp(m_CurSpeed + dir * acceSpeed * Time.deltaTime, -maxSpeed, maxSpeed);
                //3.计算目标点位
                return self + new Vector3(m_CurSpeed*Time.deltaTime,0);              
            }
            return self;
        }
        else
        {
            float a = x ? Mathf.Lerp(self.x, target.x + offset.x, smooth == 0 ? 1 : Time.deltaTime / smooth) : self.x;
            float b = y ? Mathf.Lerp(self.y, target.y + offset.y, smooth == 0 ? 1 : Time.deltaTime / smooth) : self.y;
            float c = z ? Mathf.Lerp(self.z, target.z + offset.z, smooth == 0 ? 1 : Time.deltaTime / smooth) : self.z;
            return new Vector3(a, b, c);
        }
     
    }

}
