using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace Base;

public class Program
{
    private static void Main(string[] args)
    {
        var ship1 = new Критовик();
        var ship2 = new Танк();
        var ship3 = new Ловкач();
        var arbiter = new FightArbiter(ship1, ship2, ship3);
        arbiter.Fight();
    }
}
public class FightArbiter

{
    private readonly BaseShip _first;
    private readonly BaseShip _second;
    private readonly BaseShip _third;
    public FightArbiter(BaseShip first, BaseShip second, BaseShip third)
    {
        _first = first;
        _second = second;
        _third = third;
    }
    private (bool vision, bool fight) AttackTick(BaseShip a, BaseShip b, BaseShip c, bool discovery)
    {

        if (!discovery)
        {
            var visionAB = a.VisionRange >= b.VisibilityRange;
            var visionAC = a.VisionRange >= c.VisibilityRange;
            if (!visionAB || !visionAC)
            {
                return (false, false);
            }
            var visionBA = b.VisionRange >= a.VisibilityRange;
            var visionBC = b.VisionRange >= c.VisibilityRange;
            if (!visionBA || !visionBC)
            {
                return (false, false);
            }
            var visionCA = c.VisionRange >= a.VisibilityRange;
            var visionCB = c.VisionRange >= b.VisibilityRange;
            if (!visionCA && !visionCB)
            {
                return (false, false);
            }
        }
        var InstaDeathAB = a.Damage >= (b.Shield + b.HP);
        var InstaDeathAC = a.Damage >= (c.Shield + c.HP);
        if (InstaDeathAB && InstaDeathAC)
        {
            b.HP = 0;
            Console.WriteLine($"{a.Name} win, {b.Name} and {c.Name} destroyed");
            return (true, true);
        }
        var InstaDeathBA = b.Damage >= (a.Shield + a.HP);
        var InstaDeathBC = b.Damage >= (c.Shield + c.HP);
        if (InstaDeathBA && InstaDeathBC)
        {
            return (true, true);
        }
        var InstaDeathCA = c.Damage >= (a.Shield + a.HP);
        var InstaDeathCB = c.Damage >= (b.Shield + b.HP);
        if (InstaDeathCA && InstaDeathCB)
        {
            return (true, true);
        }
        var AvialableShieldBmA = (b.Shield) - (a.Damage);
        var AvialableShieldCmA = (c.Shield) - (a.Damage);
        if (AvialableShieldBmA >= 0 && AvialableShieldCmA >= 0)
        {
            c.Shield = AvialableShieldCmA;
            b.Shield = AvialableShieldBmA;
            return (true, true);
        }
        var AvialableShieldAmB = a.Shield - b.Damage;
        var AvialableShieldCmB = c.Shield - b.Damage;
        if (AvialableShieldAmB >= 0 && AvialableShieldCmB >= 0)
        {
            c.Shield = AvialableShieldCmB;
            a.Shield = AvialableShieldAmB;
            return (true, true);
        }
        var AvialableShieldAmC = a.Shield - c.Damage;
        var AvialableShieldBmC = b.Shield - c.Damage;
        if (AvialableShieldAmC >= 0 && AvialableShieldBmC >= 0)
        {
            b.Shield = AvialableShieldBmC;
            a.Shield = AvialableShieldAmC;
            return (true, true);
        }
        var AdditionalHpDamageBmA = AvialableShieldBmA;
        var AdditionalHpDamageCmA = AvialableShieldCmA;
        var AdditionalHpDamageAmB = AvialableShieldAmB;
        var AdditionalHpDamageCmB = AvialableShieldCmB;
        var AdditionalHpDamageAmC = AvialableShieldAmC;
        var AdditionalHpDamageBmC = AvialableShieldBmC;
        var HpAvialableBmA = b.HP - AdditionalHpDamageBmA;
        var HpAvialableCmA = c.HP - AdditionalHpDamageCmA;
        var HpAvialableAmB = a.HP - AdditionalHpDamageAmB;
        var HpAvialableCmB = c.HP - AdditionalHpDamageCmB;
        var HpAvialableAmC = a.HP - AdditionalHpDamageAmC;
        var HpAvialableBmC = b.HP - AdditionalHpDamageBmC;
        if (HpAvialableBmA <= 0 && HpAvialableCmA <= 0)
        {
            c.HP = 0;
            b.HP = 0;
            Console.WriteLine($"{a.Name} winning, {b.Name} and {c.Name} destroyed");
            return (false, false);
        }
        if (HpAvialableAmB <= 0 && HpAvialableCmB <= 0)
        {
            c.HP = 0;
            a.HP = 0;
            Console.WriteLine($"{b.Name} winning, {a.Name} destroyed, {c.Name} is alive");
            return (false, false);
        }
        if (HpAvialableAmC <= 0 && HpAvialableBmC <= 0)
        {
            b.HP = 0;
            a.HP = 0;
            Console.WriteLine($"{c.Name} winning, {a.Name} destroyed, {b.Name} is alive");
            return (true, true);
        }
        b.HP = HpAvialableBmC - HpAvialableBmA;
        c.HP = HpAvialableCmB - HpAvialableCmA;
        a.HP = HpAvialableAmB - HpAvialableAmC;
        return (true, true);

    }
    public void Fight()
    {
        var random = new Random();
        var begin = random.Next(0, 3);
        var FirstAttack1 = begin == 0;
        var FirstAttack2 = begin == 1;
        var FirstAttack3 = begin == 2;
        var AttackOrder1 = FirstAttack1;
        var AttackOrder2 = FirstAttack2;
        var AttackOrder3 = FirstAttack3;
        var FirstAttackHappened = false;

        while (_first.HP >= 0 && _second.HP >= 0 && _third.HP >= 0)
        {
            if (AttackOrder1)
            {
                var (vision, fight) = AttackTick(_first, _second, _third, FirstAttackHappened);
                _second.UltimateSkill(_first);
                _third.UltimateSkill(_first);
                FirstAttackHappened = vision;
                if (!vision)
                {
                    _first.Move();
                }
                Console.WriteLine($"{_first.Name} attacked {_second.Name} and {_third.Name} , {_second.Name} HP left {_second.HP}, Shield left {_second.Shield}, {_third.Name} HP left {_third.HP}, Shield left {_third.Shield} ");
            }
            if (!AttackOrder2)
            {
                var (vision, fight) = AttackTick(_second, _first, _third, FirstAttackHappened);
                _first.UltimateSkill(_second);
                _first.UltimateSkill(_third);
                FirstAttackHappened = vision;
                if (!vision)
                {
                    _second.Move();
                }
                Console.WriteLine($"{_second.Name} attacked {_first.Name} and {_third.Name} , {_first.Name} HP left {_first.HP}, Shield left {_first.Shield}, {_third.Name} HP left {_third.HP}, Shield left {_third.Shield} ");
            }
            if (!AttackOrder3)
            {
                var (vision, fight) = AttackTick(_third, _first, _second, FirstAttackHappened);
                _second.UltimateSkill(_third);
                _first.UltimateSkill(_third);
                FirstAttackHappened = vision;
                if (!vision)
                {
                    _third.Move();
                }
                Console.WriteLine($"{_third.Name} attacked {_first.Name} and {_second.Name} , {_first.Name} HP left {_first.HP}, Shield left {_first.Shield}, {_second.Name} HP left {_second.HP}, Shield left {_second.Shield} ");
            }
            AttackOrder1 = !AttackOrder1;
            AttackOrder2 = !AttackOrder2;
            AttackOrder3 = !AttackOrder3;
            Console.WriteLine($"FightLog {_first.Name} HP left {_first.HP} Shield Left {_first.Shield} , {_second.Name} HP left {_second.HP}, Shield left {_second.Shield}, {_third.Name} HP left {_third.HP}, Shield left {_third.Shield}");
        }
    }
}
public class BaseShip
{
    public void Move()
    {
        VisibilityRange -= Speed;
        VisionRange += Speed;
    }
    public virtual void UltimateSkill(BaseShip target)
    {

    }
    public string Name { get; set; }
    public int Speed { get; set; }
    public int Damage { get; set; }
    public int HP { get; set; }
    public int Shield { get; set; }
    public int VisionRange { get; set; }
    public int VisibilityRange { get; set; }
}
public class Критовик : BaseShip
{
    public Критовик()
    {
        Name = "Критовик";
        Speed = 10;
        HP = 1000;
        Shield = 100;
        Damage = 20;
        VisibilityRange = 100;
        VisionRange = 200;
    }
}
public class Танк : BaseShip
{
    public Танк()
    {
        Name = "Танк";
        Speed = 15;
        HP = 1200;
        Shield = 100;
        Damage = 10;
        VisibilityRange = 80;
        VisionRange = 250;
    }
}
public class Ловкач : BaseShip
{
    public Ловкач()
    {
        Name = "Ловкач";
        Speed = 30;
        HP = 850;
        Shield = 100;
        Damage = 25;
        VisibilityRange = 50;
        VisionRange = 300;
    }
}