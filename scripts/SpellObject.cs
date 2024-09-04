using Godot;
using ProjectDuhamel.models.spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDuhamel.scripts
{
    public partial class SpellObject : CharacterBody2D
    {
        public SpellData spellData { get; set; }
        public BaseSpellObjectGraphics spellGraphics { get; set; } = new BaseSpellObjectGraphics();

        public float SpellObjectRotationAngle { get; set; } = 0.0f;
        public Texture2D SpellObjectTexture { get; set; } = new Texture2D();

        public Vector2 CurrentPosition { get; set; } = new Vector2();
        public Vector2 LastPosition { get; set; } = new Vector2();

        public int[] CollisionLayer { get; set; }
        public int[] CollisionMaskValues { get; set; }

        public Utilities.Directions Direction { get; set; } = Utilities.Directions.DIR_NONE;
        public Vector2 DirectionUnitVector { get; set; } = new Vector2();
        public Vector2 DirectionVector { get; set; }
        public RectangleShape2D BodyShape { get; set; } = new RectangleShape2D();

        // the location of the tileset image
        public string AssetPath { get; set; } = string.Empty;

        /// <summary>
        /// Used to initialize data for the object after it has been instantiated since you can't call a
        /// constructor with parameters.  Call this immediately after instantiation of the object.
        /// </summary>
        /// <param name="position">global position of the object</param>
        /// <param name="dir_vector">direction that the object is moving -- station is new Vector2(0,0)</param>
        /// <param name="graphics">The graphic data from the spell in spell manager</param>
        /// <param name="spell_data">The spell data from the spells in spell manager</param>
        /// <param name="name">The name of the spell in GODOT</param>
        public void Initialize(string name, Vector2 position, SpellData spell_data, BaseSpellObjectGraphics graphics,
            Vector2 dir_vector, int[] collision_layer_values, int[] collision_mask_values)
        {
            this.Name = name;
            this.spellData = spell_data;
            this.spellGraphics = graphics;
            this.Position = position;

            // Set the sprite texture
            Sprite2D sprite = GetNode<Sprite2D>("SpellObjectSprite");
            sprite.Texture = spellGraphics.GetTextureRandom();
            sprite.Centered = true;

            // Set the collision body shape
            CollisionShape2D collision_shape = GetNode<CollisionShape2D>("CollisionShape2D");
            collision_shape.Shape = spellData.SpellShape;

            // set the collision layers in GODOT
            CollisionLayer = collision_layer_values;
            CollisionMaskValues = collision_mask_values;

            // set the vectors of the object
            DirectionVector = dir_vector;
            DirectionUnitVector = dir_vector.Normalized();
            Direction = Utilities.GetDirection_9WAY(DirectionUnitVector);

            // determine the angle made by the unit vector and apply it to the rotation of this Node object in Godot.
            SpellObjectRotationAngle = (float)Math.Atan2(dir_vector.Y, dir_vector.X);
            this.Rotation = SpellObjectRotationAngle;

            // set the velocity of the object
            this.Velocity = spellData.SpellSpeed * DirectionUnitVector;

            // Set the collision layers in GODOT
            SetCollisionLayerValue(1, false); // turn off the default layer
            foreach (int value in collision_layer_values)
            {
                SetCollisionLayerValue(value, true); // and set our own
            }

            // Set the collision mask in GODOT
            SetCollisionMaskValue(1, false);  // turn of the defaults mask
            foreach (int value in collision_mask_values)
            {
                SetCollisionMaskValue(value, true); // and set our own
            }
        }

        public override void _Ready()
        {

        }

        public override void _PhysicsProcess(double delta)
        {
            this.Rotation = SpellObjectRotationAngle;

            // Update the velocity just in case....
            Velocity = spellData.SpellSpeed * DirectionUnitVector;

            var collision = MoveAndCollide(Velocity * (float)delta);

            if (collision != null)
            {

                //GD.Print("Spell Object collided with " + collision.GetCollider() + " at " + GlobalPosition);

                var collider_obj = collision.GetCollider();

                if (collision.GetCollider() is MonsterObject)
                {
                    var monster_obj = (MonsterObject)collider_obj;
                    var monster_data = monster_obj.monsterData.Copy();  // copy the data in case the monster dies in the next line

                    // Damage the monster
                    // this line can kill the mosnter and remove it from game -- so use the copied data after this
                    monster_obj.TakeDamage(Utilities.GetRandomNumber(spellData.SpellMinDamage, spellData.SpellMaxDamage));

                    GD.Print("Spell hit a monster");

                    // check if we killed the monster
                    if (monster_obj == null || monster_obj.monsterData.MonsterHitPoints <= 0)
                    {
                        Player player = GetNode<Player>("../../Player") as Player;
                        //GD.Print("player killed the monster");
                        //GD.Print("exp earned: " + monster_data.MonsterExp);

                        //this.Experience += monster_data.MonsterExp;
                        //int index = (int)monster_data.MonsterRank;
                        //GD.Print("index: " + index + "    Rank killed: " + monster_data.MonsterRank);

                        //GD.Print("Experience: " + this.Experience);
                        //GD.Print("Kill history[index]: " + this.KillHistory[index]);
                        //GD.Print("FullKill history: " + this.KillHistory[0] + "," + this.KillHistory[1] + "," +
                        //    this.KillHistory[2] + "," + this.KillHistory[3] + "," + this.KillHistory[4]);

                        player.UpdateExperienceAndHistory_FromMonsterKill(monster_data);

                    }




                }
                else if (collision.GetCollider() is RoomObject)
                {
                    GD.Print("Spell hit a room object");
                }
                else if (collision.GetCollider() is Player)
                {
                    GD.Print("Spell hit a player");
                }
                else
                {
                    GD.Print("Spell hit something else");
                }

                this.QueueFree();
            }
        }
    }
}
