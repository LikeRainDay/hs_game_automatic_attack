using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 战后道具选择界面元素
/// </summary>
public class Item4 : MonoBehaviour
{
    [FieldName("图标")]
    public Image Icon;
    [FieldName("名字")]
    public TransMeshPro Name;
    [FieldName("类型")]
    public TransMeshPro Type;
    [FieldName("效果")]
    public TransMeshPro Effect;
    [FieldName("回收价格")]
    public TransMeshPro Price;
    private int m_ID;
    public int ID
    {
        get => m_ID;
        set
        {
            m_ID = value;
            //刷新内容
            RowCfgItem row = ConfigManager.Instance.cfgItem[value];
            GetComponent<ColorTool>().Rank = row.rank;//根据品质来刷新颜色
            Icon.SetSpriteAsync(row.icon);//设置图标
            Name.UpdateTrans(row.name);//设置名字
            Type.UpdateTrans(row.markTextID);//设置类型
            string effect = ConfigManager.Instance.cfgText[row.effectIntroduce].contents[GameManager.Instance.CurLanguage.GetHashCode()];
            //设置效果文本
            Effect.text = Player.Instance.ItemContainer.TryCalcString(effect, true).Replace("[", "").Replace("]", "").Replace("~", "#").Replace("\\n", "\n");

            //设置回收价格           
            Price.UpdateTrans(Price.TextId, Mathf.RoundToInt(row.price * Player.Instance.ItemsRecyclePrice * (1 + Player.Instance.CurWave / 10)).ToString());
        }
    }

    /// <summary>
    /// 拿取
    /// </summary>
    public void Take()
    {
        Player.Instance.ItemContainer.AddItem(ID);
        Next();
    }

    /// <summary>
    /// 回收
    /// </summary>
    public void Recycle()
    {
        Player.Instance.MaterialChange(Mathf.RoundToInt(ConfigManager.Instance.cfgItem[ID].price * Player.Instance.ItemsRecyclePrice) * (1 + Player.Instance.CurWave / 10));
        Next();
    }

    /// <summary>
    /// 下一个道具
    /// </summary>
    public async void Next()
    {
        Player.Instance.RewardIds.RemoveAt(0);
        //如果数组里还有元素 就显示下一个道具
        if (Player.Instance.RewardIds.Count > 0)
        {
            ID = Player.Instance.RewardIds[0];
            transform.DOScaleX(0, 0.12f).onComplete += () =>
            {
                transform.DOScaleX(1, 0.08f);
            };
        }
        else
        {
            //如果没有元素了就隐藏当前页面，如果当前波次升级过就显示Buff选择界面，否则直接进入商店界面
            ItemSelectedPage.Instance.Hide();
            if (Player.Instance.LevelUpTimes >= 0)
            {
                await Task.Delay(200);
                BuffSelectedPage.Instance.Show();
            }
            else
            {
                //隐藏遮盖场景的遮罩，然后显示商店界面
                ItemSelectedPage.Instance.Mask.SetActive(false);//隐藏当前界面和战斗界面，显示商店界面
                UIMgr.Instance.ShowPage(new List<GameObject>() { gameObject, BattlePage.Instance.gameObject }, ShopPage.Instance.gameObject, null, 0.5f, 0.25f, 0.5f);
            }
        }
    }

}
