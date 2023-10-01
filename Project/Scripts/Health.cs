using IcarianEngine;

namespace LD54
{
    public class Health : Component
    {
        public HealthDef HealthDef
        {
            get
            {
                return Def as HealthDef;
            }
        }

        float m_health;

        public float CurrentHealth
        {
            get
            {
                return m_health;
            }
            set
            {
                m_health = value;
            }
        }

        public override void Init()
        {
            HealthDef def = HealthDef;
            if (def == null)
            {
                Logger.Error("Health component requires a HealthDef");

                return;
            }

            m_health = def.MaxHealth;
        }
    }
}