namespace LD54
{
    // Have not fully implemented native stuff for Mono yet, so this is a
    // simple implementation of a psuedo random number generator.
    // Not good but it works for now.
    public static class Random
    {
        static uint s_state = 0xdeadbeef;

        public static uint GetRandomUInt()
        {
            // Using primes as prime numbers seem to be good for random number generation
            s_state ^= s_state << 13;
            s_state ^= s_state >> 17;
            s_state ^= s_state << 5;

            return s_state;
        }

        public static uint GetRandomUInt(uint a_min, uint a_max)
        {
            return a_min + (GetRandomUInt() % (a_max - a_min));
        }

        public static float GetRandomFloat()
        {
            return (float)(GetRandomUInt() / (double)uint.MaxValue);
        }
        public static float GetRandomFloat(float a_min, float a_max)
        {
            return a_min + (GetRandomFloat() * (a_max - a_min));
        }

        public static void UpdateState()
        {
            s_state ^= s_state << 11;
            s_state ^= s_state >> 7;
            s_state ^= s_state << 17;
        }
    }
}