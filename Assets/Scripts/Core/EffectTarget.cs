using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ч������Ŀ�������ö��
/// </summary>
public enum EffectTarget
{
    [EnumName("����")]
    Self = 0,
    [EnumName("�Ѿ�")]
    Partner = 10,
    [EnumName("�Ѿ�������������")]
    PartnerNotContainSelf = 11,
    [EnumName("�о�")]
    Enemy = 20,
    [EnumName("ȫ��")]
    All = 30,
    [EnumName("ȫ��������������")]
    AllNotContainSelf = 31,
}
