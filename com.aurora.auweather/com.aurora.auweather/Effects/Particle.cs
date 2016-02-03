using System;
using System.Numerics;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.Effects
{
    // 粒子类是粒子系统的组成元素，一个粒子系统拥有大量的粒子，它们拥有一些基本的物理特性。
    // 例如位置、速度、加速度以及旋转。它们将以精灵方式绘制，悬浮在画布之上，会非常漂亮。
    public class Particle
    {
        // 位置、速度、加速度三个公开字段的 .X 和 .Y 属性能被用户直接访问
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        // 粒子的存活周期
        public float Lifetime;

        // 粒子从初始化到现在的持续时间
        public float TimeSinceStart;

        // 缩放倍率
        public float Scale;

        // 旋转弧度
        public float Rotation;

        // 旋转速度
        public float RotationSpeed;


        // 初始化方法被粒子系统调用，用来准备合适的粒子
        public void Initialize(Vector2 position, Vector2 velocity, Vector2 acceleration, float lifetime, float scale, float rotation, float rotationSpeed)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = acceleration;
            this.Lifetime = lifetime;
            this.Scale = scale;
            this.RotationSpeed = rotationSpeed;

            // 重置生成时间
            this.TimeSinceStart = 0.0f;

            // 初始化旋转角度
            this.Rotation = rotation;
        }


        // 每次刷新时，Update 方法都会由粒子系统调用，位置等参数都将被更新，并且返回此粒子是否存活的 bool 值
        public bool Update(float elapsedTime)
        {
            Velocity += Acceleration * elapsedTime;
            Position += Velocity * elapsedTime;

            Rotation += RotationSpeed * elapsedTime;

            TimeSinceStart += elapsedTime;

            return TimeSinceStart < Lifetime;
        }
    }
}
