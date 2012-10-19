//#define DEBUG_WALKING
using System;
using System.Collections.Generic;
using Common.IMotion;
using SlimDX;

namespace Common.Motion
{
    [Serializable]
    public class Unit : Object, IUnit, ISweepAndPruneObject
    {
        public Unit()
        {
            Weight = 1f;
            TurnSpeed = 5f;
            CurrentPhysicsState = PhysicsState.Unknown;
        }

        public override void Step(float dtime)
        {
            //Simulation.USMiscStart();

            base.Step(dtime);

            intersectingUnits.Clear();
            IntersectedUnits = false;

            InterpolatedRotation = Quaternion.Slerp(InterpolatedRotation, Rotation, TurnSpeed * dtime);

#if BETA_RELEASE
            Simulation.USFindStateStart();
#endif
            if (CurrentPhysicsState == PhysicsState.Unknown)
                CurrentPhysicsState = FindCurrentPhysicsState();
#if BETA_RELEASE
            Simulation.USFindStateStop();
#endif

            if (velocityImpulse != Vector3.Zero)
            {
                Velocity = velocityImpulse;
                velocityImpulse = Vector3.Zero;
                if (Velocity.Z != 0)
                {
                    CurrentPhysicsState = PhysicsState.InAir;
                    position = new Vector3(position.X, position.Y, position.Z + 0.01f);
                }
            }
            else
            {
                if (IsOnGround)
                {
                    //Vector2 v = Common.Math.ToVector2(Velocity);
                    //float speed = v.Length();
                    //RunVelocity = Vector2.Normalize(v + dtime * SteerVelocity) * speed;
                    //Velocity = new Vector3(RunVelocity, 0) * speed;
                    Velocity = new Vector3(RunVelocity, 0);
                }
                else
                    Velocity = Velocity - new Vector3(0, 0, dtime * Simulation.Settings.Gravity);
            }

            //Simulation.USMiscStop();

            if (Velocity == Vector3.Zero && !Simulation.ForcedUpdate)
                return;

            //PhysicsState lastState = CurrentPhysicsState;

            Vector3 intersection;
            Vector3 newPosition = position + dtime * Velocity;

#if DEBUG_WALKING
            stepHistory = new StepHistory();
            stepHistory.InitialPosition = position;
            stepHistory.WantedPosition = newPosition;
            stepHistory.Velocity = Velocity;
            stepHistory.InitialState = CurrentPhysicsState;
#endif

            if (CurrentPhysicsState == PhysicsState.OnGround)
            {
#if BETA_RELEASE
                Simulation.USWalkStart();
#endif
                CurrentPhysicsState = CalcWalkability(position, Common.Math.ToVector2(newPosition - position), out intersection);
                if (CurrentPhysicsState == PhysicsState.InAir)
                    Velocity = new Vector3(Velocity.X, Velocity.Y, -lastDescentSpeed);
                else if (intersection.Z < position.Z)
                    lastDescentSpeed = (position.Z - intersection.Z) / dtime;
                else
                    lastDescentSpeed = 0;
#if BETA_RELEASE
                Simulation.USWalkStop();
#endif
            }
            else if (CurrentPhysicsState == PhysicsState.InAir)
            {
#if BETA_RELEASE
                Simulation.USFlyStart();
#endif
                CurrentPhysicsState = CalcFlyability(position, newPosition - position, out intersection);
                //flyingInfo.Add(new DebugOutput { Position = intersection, Diff = newPosition - position, Velocity = Velocity });
                //if (CurrentPhysicsState == PhysicsState.OnGround)
                //{
                //    string s = "";
                //    foreach (var d in flyingInfo)
                //        s += d + Environment.NewLine;
                //    System.Windows.Forms.MessageBox.Show(s);
                //    flyingInfo.Clear();
                //}
#if BETA_RELEASE
                Simulation.USFlyStop();
#endif
            }
            else
                throw new Exception("Buggy code");

            //Simulation.USMiscStart();
            if (position != intersection)
            {
                position = intersection;
                base.Position = intersection;
            }
            //Simulation.USMiscStop();
#if DEBUG_WALKING
            stepHistory.FinalPosition = position;
            stepHistory.FinalState = CurrentPhysicsState;
            history.Add(stepHistory);
            if (history.Count > 300)
                history.RemoveAt(0);
            if (position.X <= 14)
                Console.WriteLine("asd");
#endif
        }

        //private List<DebugOutput> flyingInfo = new List<DebugOutput>();

        //private class DebugOutput
        //{
        //    public Vector3 Position { get; set; }
        //    public Vector3 Velocity { get; set; }
        //    public Vector3 Diff { get; set; }
        //    private string VectorString(Vector3 v) { return String.Format("({0:+0.000;-0.000}, {1:+0.000;-0.000}, {2:+0.000;-0.000})", v.X, v.Y, v.Z); } 
        //    public override string ToString()
        //    {
        //        return "Pos: " + VectorString(Position) + "; Vel: " + VectorString(Velocity) + "; Diff: " + VectorString(Diff);
        //    }
        //}

        private PhysicsState FindCurrentPhysicsState()
        {
            float d;
            Ray r = new Ray(position + new Vector3(0, 0, 0.005f), -Vector3.UnitZ);

            if (Simulation.StaticObjectsProbe.Intersect(r, out d))
            {
                // If the distance is very small we can assume that we are on ground. This method is only supposed to be called 
                // when too many changes have occured to the object so it shouldn't interfere with jump and such.
                if (d < 0.05f && Velocity.Z <= 0)
                {
                    position = r.Position + d * r.Direction;
                    return PhysicsState.OnGround;
                }
                else
                    return PhysicsState.InAir;
            }
            else
            {
                // We are currently below ground! Try to find some place safe to stand above
                r = new Ray(position + 50f * Vector3.UnitZ, -Vector3.UnitZ);
                if (Simulation.StaticObjectsProbe.Intersect(r, out d))
                {
                    position = r.Position + d * r.Direction;
                    return PhysicsState.OnGround;
                }
                else
                    return PhysicsState.InAir;
            }
        }

        private PhysicsState CalcFlyability(Vector3 position, Vector3 diff, out Vector3 intersection)
        {
            bool hit;
            float d;
            Object obj;

            DebugOutput(String.Format("CalkFlyability running (pos: {0}, diff: {1})", VectorToString(position), VectorToString(diff)));
            if (Common.Math.ToVector2(diff).Length() > 0)
                hit = LimitedIntersect(position, diff, out d);
            else if (diff.Z < 0)
                hit = Simulation.StaticObjectsProbe.BVH.Intersect(new Ray(position, Vector3.Normalize(diff)), out d, out obj);
            else        // vertical jump, assume nothing is above
            {
                intersection = position + diff;
                return PhysicsState.InAir; 
            }

            if (hit)
            {
                Ray r = new Ray(position, new Vector3(0, 0, diff.Z > 0 ? 1 : -1));
                hit = Simulation.StaticObjectsProbe.Intersect(r, out d);

                if (hit && d < System.Math.Abs(diff.Z))
                {
                    // this conditional statement could be looked into some more...
                    if (diff.Z < 0)
                    {
                        intersection = r.Position + d * r.Direction;
                        return PhysicsState.OnGround;
                    }
                    else
                    {
                        intersection = position;
                        ///* Error check */
                        //r = new Ray(new Vector3(intersection.X, intersection.Y, 50), -Vector3.UnitZ);
                        //hit = Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj);
                        //Vector3 newPos = r.Position + d * r.Direction;
                        //if (hit && intersection.Z <= newPos.Z)
                        //    System.Diagnostics.Debugger.Break();
                        ///* End of Error Check */
                        return PhysicsState.InAir;
                    }
                }
                else
                {
                    intersection = position + new Vector3(0, 0, diff.Z);
                    return PhysicsState.InAir;
                }
            }
            else
            {
                intersection = position + diff;
                /* HACK SO PEOPLE DON'T FALL THROUGH THE GROUND. FIX PROPERLY! */
                if (Simulation.DebugReturnQuadtree.OptimizeGroundIntersection)
                {
                    float groundHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                    if (intersection.Z < groundHeight)
                    {
                        float newZ = System.Math.Max(position.Z + diff.Z, Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(position));
                        intersection = new Vector3(position.X, position.Y, newZ);
                        return PhysicsState.OnGround;
                    }
                }
                /* END OF HACK */

                ///* Error check */
                //Ray r = new Ray(new Vector3(intersection.X, intersection.Y, 50), -Vector3.UnitZ);
                //hit = Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj);
                //Vector3 newPos = r.Position + d * r.Direction;
                //if (hit && intersection.Z <= newPos.Z)
                //{
                //    float oldHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(position);
                //    float newHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                //    System.Diagnostics.Debugger.Break();
                //    oldHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(position);
                //    newHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                //    r = new Ray(position, Vector3.Normalize(diff));
                //    hit = Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj);
                //    newPos = r.Position + d * r.Direction;
                //    newHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                //}
                ///* End of Error Check */
                return PhysicsState.InAir;
            }
        }

        private PhysicsState CalcWalkability(Vector3 position, Vector2 diff, out Vector3 intersection)
        {
            bool hit;
            float d;

            DebugOutput(String.Format("CalkWalkability running (pos: {0}, diff: {1})", VectorToString(position), VectorToString(new Vector3(diff, 0))));
            Ray r = new Ray(position + new Vector3(diff, 50f), -Vector3.UnitZ);         // ray from high above
            hit = Simulation.StaticObjectsProbe.Intersect(r, out d);
            d = (float)System.Math.Round(d, 4);     // hack

            if (hit)
            {
                Vector3 newPos = r.Position + d * r.Direction;

                // check angle of intersection to see if we can travel to the new point
                if (position.Z > newPos.Z)      // slope
                {
                    //float theta = (float)System.Math.Atan((position.Z - newPos.Z) / diff.Length());
                    //if (theta > Simulation.Settings.MaxDescendAngle)
                    float allowedHeight = AllowedStepdown(diff);
                    if (position.Z - newPos.Z > allowedHeight)
                    {
                        // fall
                        intersection = position + new Vector3(diff, 0);

                        ///* Error check */
                        //Object obj;
                        //r = new Ray(new Vector3(intersection.X, intersection.Y, 50), -Vector3.UnitZ);
                        //hit = Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj);
                        //Vector3 np = r.Position + d * r.Direction;
                        //if (hit && intersection.Z <= np.Z)
                        //{
                        //    float oldHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(position);
                        //    float newHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                        //    System.Diagnostics.Debugger.Break();
                        //    oldHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(position);
                        //    newHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                        //    r = new Ray(position, Vector3.Normalize(Common.Math.ToVector3(diff)));
                        //    hit = Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj);
                        //    np = r.Position + d * r.Direction;
                        //    newHeight = Simulation.DebugReturnQuadtree.DebugCheckGroundHeight(intersection);
                        //}
                        ///* End of Error Check */

                        return PhysicsState.InAir;
                    }
                    else
                    {
                        // walk on ground
                        intersection = newPos;
                        return PhysicsState.OnGround;
                    }
                }
                else                            // hill or even ground
                {
                    // NOTE: This is not a very good solution. The maximum steepness of traversable hills varies with user speed
                    if (newPos.Z - position.Z < Simulation.Settings.MaxStepHeight)      // if not too high...
                        intersection = newPos;                      // ... traverse
                    else
                    {
                        if (diff.Length() == 0)     // hack: fix for new intersection meshes spawned upon the unit
                        {
                            //Console.WriteLine("Unit stuck!");
                            intersection = position;
                            return PhysicsState.OnGround;
                        }

                        /* Slide fix starts here */
                        
                        float slideDistance;
                        Vector3 outDirection;
                        Vector3 origin = position + new Vector3(0, 0, Simulation.Settings.MaxStepHeight);

#if BETA_RELEASE
                        Simulation.WalkSlideStart();
#endif
                        if (WillSlide(origin, Common.Math.ToVector3(diff), out outDirection, out slideDistance))
                        {
                            DebugOutput(String.Format("WillSlide: True (outDir: {0})", VectorToString(outDirection)));
                            //Vector2 diffRemains = (diff.Length() * (1f - slideDistance)) * diff;   // what remains of the vector after we hit the sliding wall
                            Vector2 diffRemains = diff;
                            float x = Vector3.Dot(Common.Math.ToVector3(diffRemains), outDirection);
                            if (x < 0)
                            {
#if BETA_RELEASE
                                Simulation.WalkSlideStop();
#endif

                                DebugOutput(String.Format("CalkWalkability: Negative dot product (x = {0})", x));
                                intersection = position;
                                return PhysicsState.OnGround;
                            }

                            // try to travel x * outDirection
                            r = new Ray(origin, outDirection);
                            Object obj;
                            if (LimitedIntersect(origin, outDirection * x, out d))
                            //if (Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj) && d < x)
                            {
#if BETA_RELEASE
                                Simulation.WalkSlideStop();
#endif

                                DebugOutput(String.Format("CalkWalkability: Traversal obstacle hit (d: {0}, x: {1})", d, x));
                                intersection = position;
                                return PhysicsState.OnGround;
                            }
                            else
                            {
                                DebugOutput(String.Format("CalkWalkability: No traversal obstacle (d: {0}, x: {1})", d, x));
                                newPos = r.Position + x * r.Direction;
                                r = new Ray(newPos, -Vector3.UnitZ);

                                if (Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj) && d <= Simulation.Settings.MaxStepHeight + 0.1f)        // allow stepping down a bit without falling
                                {
#if BETA_RELEASE
                                    Simulation.WalkSlideStop();
#endif

                                    DebugOutput(String.Format("CalkWalkability: NewPos ground hit (newPos: {0}, d: {1})", newPos, d));
                                    intersection = r.Position + d * r.Direction;
                                    return PhysicsState.OnGround;
                                }
                                else
                                {
#if BETA_RELEASE
                                    Simulation.WalkSlideStop();
#endif

                                    DebugOutput(String.Format("CalkWalkability: NewPos ground miss (newPos: {0}, d: {1}, maxH: {2})", newPos, d, Simulation.Settings.MaxStepHeight));
                                    intersection = r.Position - new Vector3(0, 0, Simulation.Settings.MaxStepHeight);
                                    return PhysicsState.OnGround;       // should be InAir... make sure there are no other bugs around first
                                }
                            }
                        }
                        else
                        {
#if BETA_RELEASE
                            Simulation.WalkSlideStop();
#endif
                            DebugOutput(String.Format("WillSlide: False (outDir: {0}, slideD: {1})", VectorToString(outDirection), slideDistance));
                            if (outDirection.Length() == 0)
                                System.Diagnostics.Debugger.Break();    // should not happen

#if DEBUG_WALKING
                            if (slideDistance > diff.Length())
                                System.Diagnostics.Debugger.Break();
#endif

                            Ray ray = new Ray(origin + slideDistance * outDirection, -Vector3.UnitZ);       // this may need to be adjusted for rounding errors
                            float dOut;
                            Object obj;
                            if (Simulation.StaticObjectsProbe.BVH.Intersect(ray, out dOut, out obj) && dOut <= Simulation.Settings.MaxStepHeight + 0.1f)     // allow stepping down a bit without falling
                            {
                                intersection = ray.Position + dOut * ray.Direction;
                                return PhysicsState.OnGround;
                            }
                            else
                            {
                                intersection = ray.Position - new Vector3(0, 0, Simulation.Settings.MaxStepHeight);
                                return PhysicsState.InAir;
                            }
                        }

                        /* Slide fix ends here */
                    }
                    return PhysicsState.OnGround;
                }
            }
            else
            {
                DebugOutput(String.Format("CalkWalkability: Missed the ground"));
                intersection = position + Common.Math.ToVector3(diff);
                return PhysicsState.InAir;
            }
        }

        // Only applies when method returns true: middleLength is used to find the distance to the middle point 
        // of our approximated sliding wall so we can reduce the diff-vector properly
        private bool WillSlide(Vector3 origin, Vector3 direction, out Vector3 outDirection, out float slideDistance)
        {
            DebugOutput(String.Format("WillSlide running (orig: {0}, dir: {1})", VectorToString(origin), VectorToString(direction)));
            outDirection = Vector3.Zero;
            slideDistance = 0;
            Vector2 diff = Common.Math.ToVector2(direction);
            float diffLength = diff.Length();
            if (diffLength == 0)
                return false;
            DebugOutput(String.Format("WillSlide: diffLength = {0}", diffLength));

            float theta = (float)Common.Math.AngleFromVector3XY(new Vector3(diff, 0));
            float theta1 = theta - 0.01f;
            float theta2 = theta + 0.01f;
            Vector3 dir1 = Common.Math.Vector3FromAngleXY(theta1);
            Vector3 dir2 = Common.Math.Vector3FromAngleXY(theta2);
            dir1.Z = dir2.Z = outDirection.Z;
            
            Ray r1 = new Ray(origin, dir1);
            Ray r2 = new Ray(origin, dir2);
            float d1, d2;
            Object obj;

            float limitedD1, limitedD2;
            bool limitedHit1 = LimitedIntersect(r1.Position, dir1 * diff.Length(), out limitedD1);
            bool limitedHit2 = LimitedIntersect(r2.Position, dir2 * diff.Length(), out limitedD2);
            bool hit1, hit2;
            //hit1 = Simulation.StaticObjectsProbe.BVH.Intersect(r1, out d1, out obj) && d1 <= diffLength;
            //hit2 = Simulation.StaticObjectsProbe.BVH.Intersect(r2, out d2, out obj) && d2 <= diffLength;
            hit1 = limitedHit1; d1 = limitedD1; hit2 = limitedHit2; d2 = limitedD2;
            if (hit1 && hit2)
            {
                DebugOutput(String.Format("WillSlide: Two hits ({0}, {1})", d1, d2));
                // keep d1 smallest
                if (d2 < d1)
                {
                    Common.Math.Swap<float>(ref d1, ref d2);
                    Common.Math.Swap<Ray>(ref r1, ref r2);
                }
                // create vector from shortest hit to longest
                Vector3 p1 = r1.Position + d1 * r1.Direction;
                Vector3 p2 = r2.Position + d2 * r2.Direction;
                outDirection = Vector3.Normalize(p2 - p1);
                slideDistance = (d1 + d2) / 2f;     // not entirely true to the name... really returns the distance to the interpreted middle point for the direction intersection
                return true;
            }
            else if (hit1 || hit2)
            {
                DebugOutput(String.Format("WillSlide: One hit ({0}: {1}, {2})", hit1 ? "1" : "2", hit1 ? d1 : d2, hit1 ? d2 : d1));
                slideDistance = diffLength;
                if (hit1)
                    outDirection = r2.Direction;
                else
                    outDirection = r1.Direction;
                return false;
            }
            else
            {
                DebugOutput(String.Format("WillSlide: No hit ({0}, {1})", d1, d2));
                outDirection = r1.Direction;        // just pick one
                slideDistance = diffLength;
                return false;
            }
        }

        public void VelocityImpulse(Vector3 impulse)
        {
            velocityImpulse = impulse;
        }

        private bool LimitedIntersect(Vector3 position, Vector3 step, out float distance)
        {
            Object obj;
            Ray r = new Ray(position, Vector3.Normalize(step));
            return Simulation.StaticObjectsProbe.BVH.Intersect(r, step.Length(), out distance, out obj);
            //Vector3 p1 = position + step;
            //var list = Simulation.StaticObjectsProbe.BVH.Cull(new Common.Bounding.Line(position, p1));
            //if (list.Count > 0)
            //{
            //    float minD = float.MaxValue;
            //    RayIntersection rOut;
            //    Ray r = new Ray(position, Vector3.Normalize(step));
            //    foreach (Object o in list)
            //    {
            //        if (Intersection.Intersect<RayIntersection>(o.WorldBounding, r, out rOut) && rOut.Distance < minD)
            //            minD = rOut.Distance;
            //    }
            //    distance = minD;
            //    if (minD < float.MaxValue)
            //        return true;
            //    else
            //        throw new Exception("Cull returned object(s) that did not pass intersection test.");
            //}
            //else
            //{
            //    distance = float.NaN;
            //    return false;
            //}
        }

        public void AddIntersectingUnit(Unit unit) 
        { 
            intersectingUnits.Add(unit); 
            if (IntersectsUnitEvent != null)
                IntersectsUnitEvent(this, new IntersectsObjectEventArgs { IObject = unit }); 
        }

        private static Random random = new Random(0);

        // writes new position to upcomingPosition
        public List<Unit> ResolveIntersections()
        {
            if (intersectingUnits.Count == 0)
                return null;

            var affectedUnits = new List<Unit>();

            var r1 = Common.Boundings.Radius(WorldBounding);
            Vector2 p1 = Common.Math.ToVector2(UpcomingPosition);
            Vector2 v1 = Common.Math.ToVector2(Velocity);
            foreach (var u in intersectingUnits)
            {
                Vector2 v2 = Common.Math.ToVector2(u.Velocity);

                var r2 = Common.Boundings.Radius(u.WorldBounding);

                Vector2 v = p1 - Common.Math.ToVector2(u.UpcomingPosition);
                if (v == Vector2.Zero)
                    v = new Vector2((float)random.NextDouble() * 0.00001f, (float)random.NextDouble() * 0.00001f);

                float distance = (r1 + r2 - v.Length()) * 1.10f;  // add 10% so we get slightly less collisions

                if (DebugSolidAsARock || u.DebugSolidAsARock)
                {
                    if (DebugSolidAsARock)
                        u.UpcomingPosition -= Common.Math.ToVector3(Vector2.Normalize(v) * distance);
                    else
                        UpcomingPosition += Common.Math.ToVector3(Vector2.Normalize(v) * distance);
                }
                else
                {
                    /*
                    // http://en.wikipedia.org/wiki/Inelastic_collision
                    float rCoeff = 0f;    // coefficient of restitution
                    float m_1 = Weight; float m_2 = u.Weight;
                    Vector2 v_1 = v1; Vector2 v_2 = v2; // these might be wrong... Wikipedia: "For two- and three-dimensional collisions the velocities in these formulas are the components perpendicular to the tangent line/plane at the point of contact."

                    //// whoo, those moving bastards can sit on something!
                    //if (v1 != v2)
                    //{
                    //    if (v1 == Vector2.Zero)
                    //        m_2 *= 0.1f;
                    //    else if (v2 == Vector2.Zero)
                    //        m_1 *= 0.1f;
                    //}

                    // v1f = (C_r * m_2 * (v_2 - v_1) + m_1*v_1 + m_2*v_2) / (m_1 + m_2)
                    // v2f = (C_r * m_1 * (v_1 - v_2) + m_1*v_1 + m_2*v_2) / (m_1 + m_2)
                    Vector2 v1f = (rCoeff * m_2 * (v_2 - v_1) + m_1 * v_1 + m_2 * v_2) / (m_1 + m_2);
                    Vector2 v2f = (rCoeff * m_1 * (v_1 - v_2) + m_1 * v_1 + m_2 * v_2) / (m_1 + m_2);

                    float scale;
                    float velocitySum = v1f.Length() + v2f.Length();
                    if (velocitySum == 0)
                        scale = 0.5f;
                    else        // use the supposed collision velocities to scale the size of the displacement vectors for the two involved units
                        scale = v1f.Length() / velocitySum;
                    */

                    float scale = u.Weight / (Weight + u.Weight);
                    UpcomingPosition += Common.Math.ToVector3(distance * scale * v);
                    u.UpcomingPosition -= Common.Math.ToVector3(distance * (1f - scale) * v);
                }
                
                if (u.intersectingUnits.Contains(this))
                    u.intersectingUnits.Remove(this);

                affectedUnits.Add(u);
                u.IntersectedUnits = true;
            }
            intersectingUnits.Clear();
            IntersectedUnits = true;
            if (affectedUnits.Count > 0)
                affectedUnits.Add(this);
            return affectedUnits;
        }

        public bool DebugSolidAsARock { get; set; }

        public bool IntersectedUnits
        {
            get { return intersectedUnits; }
            protected set { intersectedUnits = value; } 
        }

        private float AllowedStepdown(Vector2 diff)
        {
            return diff.Length() * (float)System.Math.Tan(Simulation.Settings.MaxDescendAngle);
        }

        public void CommitCollisionResolution()
        {
            bool hit;
            float d;
            Object obj;
            Ray r = new Ray(UpcomingPosition + new Vector3(0, 0, 50f), -Vector3.UnitZ);
            hit = Simulation.StaticObjectsProbe.BVH.Intersect(r, out d, out obj);
            if (hit && d >= 50f - Simulation.Settings.MaxStepHeight)
            {
                Vector2 diff = Common.Math.ToVector2(UpcomingPosition - Position);
                float allowedHeight = AllowedStepdown(diff);
                if (d > 50 + allowedHeight)
                {
                    base.Position = position = new Vector3(UpcomingPosition.X, UpcomingPosition.Y, position.Z);
                    CurrentPhysicsState = PhysicsState.InAir;
                }
                else
                {
                    base.Position = position = r.Position + d * r.Direction;
                    CurrentPhysicsState = PhysicsState.OnGround;
                }
            }
        }

        public enum PhysicsState
        {
            Unknown,
            OnGround,
            InAir,
        }

#if DEBUG_WALKING
        private class StepHistory
        {
            public Vector3 InitialPosition { get; set; }
            public Vector3 WantedPosition { get; set; }
            public Vector3 FinalPosition { get; set; }
            public Vector3 Velocity { get; set; }
            public PhysicsState InitialState { get; set; }
            public PhysicsState FinalState { get; set; }
            public List<String> Events = new List<string>();
            private String EventsString
            { 
                get 
                { 
                    string s = "";
                    foreach (var line in Events) s += line + "; ";
                    return s;
                }
            }
            public override string ToString()
            {
                return String.Format("{0} from {1} - ", VectorToString(FinalPosition), VectorToString(InitialPosition)) + EventsString;
            }
        }
        private List<StepHistory> history = new List<StepHistory>();
        private StepHistory stepHistory;
#endif

        private static String VectorToString(Vector3 v)
        {
            return String.Format("({0:0.000}, {1:0.000}, {2:0.000})", v.X, v.Y, v.Z);
        }

        private void DebugOutput(string debugString)
        { 
#if DEBUG_WALKING
            stepHistory.Events.Add(debugString);
#endif
        }

        public Vector3 UpcomingPosition { get; set; }       // used when doing unit-unit intersection'
        SlimDX.Quaternion interpolatedRotation;
        public override SlimDX.Quaternion InterpolatedRotation
        {
            get
            {
                return interpolatedRotation;
            }
            protected set
            {
                interpolatedRotation = value;
            }
        }
        //public new Quaternion InterpolatedRotation { get; private set; }

        private List<Unit> intersectingUnits = new List<Unit>();
        private bool intersectedUnits = false;

        public Vector2 RunVelocity { get; set; }
        //public Vector2 SteerVelocity { get; set; }
        public bool IsOnGround { get { return CurrentPhysicsState == PhysicsState.OnGround; } }
        public PhysicsState CurrentPhysicsState { get; private set; } 

        /// <summary>
        /// This property set is only supposed to be used when setting the position to an entirely new world point and not for advancing movement
        /// </summary>
        public new Vector3 Position
        {
            get { return position; }
            set
            {
                base.Position = value;
                position = value;
                PreviousPosition = value;
                InterpolatedPosition = value;
                CurrentPhysicsState = PhysicsState.Unknown;
                
                //if (noInitialPositionSet)
                //{
                //    PreviousPosition = value;
                //    noInitialPositionSet = false;
                //}
            }
        }
        private Vector3 position;
        public Vector3 Velocity { get; private set; }
        public Vector3 PreviousPosition { get; set; }
        public Vector3 PreviousVelocity { get; set; }
        public Vector3 InterpolatedVelocity { get; set; }
        public new Vector3 InterpolatedPosition { get; set; }

        float weight;
        public float Weight
        {
            get { return weight; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("Weight must be > 0");
                weight = value;
            }
        }
        //public event Action<IUnit> IntersectsUnit;
        public event EventHandler<IntersectsObjectEventArgs> IntersectsUnit
        {
            add { IntersectsUnitEvent += value; }
            remove { IntersectsUnitEvent -= value; }
        }

        public float Radius { get { return Common.Boundings.Radius(WorldBounding); } }
        public float TurnSpeed { get; set; }

        private event EventHandler<IntersectsObjectEventArgs> IntersectsUnitEvent;

        private Vector3 velocityImpulse;
        private float lastDescentSpeed = 0;     // used to make falls when descending steep hills look better

        #region ISweepAndPruneObject Members

        public void UpdateMinMax()
        {
            float radius = Radius;
            Min = new Vector2(position.X - radius, position.Y - radius);
            Max = new Vector2(position.X + radius, position.Y + radius);
        }

        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        public List<ISweepAndPruneObject> Intersectees
        {
            get { return intersectees; }
        }
        private List<ISweepAndPruneObject> intersectees = new List<ISweepAndPruneObject>();

        #endregion
    }
}

