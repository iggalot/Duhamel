namespace ProjectDuhamel.models.spells
{
    public enum SpellShape
    {
        SPELL_SHAPE_SINGLE_MISSILE = 0,
        SPELL_SHAPE_SINGLE_CIRCLE = 1,
    }
    public enum ElementalDamageTypes
    {
        ELEMENTAL_TYPE_NONE = -1,
        ELEMENTAL_TYPE_PHYSICAL = 0,
        ELEMENTAL_TYPE_FIRE = 1,
        ELEMENTAL_TYPE_COLD = 2,
        ELEMENTAL_TYPE_LIGHTNING = 3,
        ELEMENTAL_TYPE_POISON = 4,
        ELEMENTAL_TYPE_EARTH = 5
    }

    public enum SpellIdentifiers
    {
        SPELL_UNDEFINED = -1,
        SPELL_FIRE_BOLT = 0,
        SPELL_LIGHTNING_BOLT = 1,
        SPELL_POISON_BOLT = 2,
        SPELL_FROST_BOLT = 3,
        SPELL_EARTH_BOLT = 4
    }
    public class SpellManager
    {
    }
}
