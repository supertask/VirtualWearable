using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class HandUtil
{
    public static int LEFT = 0;
    public static int RIGHT = 1;
    private bool[] existsPreviousHands;
    private bool[] isOpenedPreviousHands;
    private bool[,] isOpenedPreviousFingers;
    private int[] handIds;
    private Transform playerTransform;

    public enum HandActionType: int { JUST_OPENED, JUST_CLOSED }
    public enum FingerActionType: int { JUST_OPENED, JUST_CLOSED }
    public enum HandStatus : int { OPEN, CLOSE, UNKNOWN }


    [Tooltip("Velocity (m/s) move toward ")]
    public float smallestVelocity = 0.4f;

    [Tooltip("Velocity (m/s) move toward ")]
    public float deltaVelocity = 0.7f; //0.7f is default

    [Tooltip("Delta degree to check 2 vectors same direction")]
    public float handForwardDegree = 30;

    public HandUtil(Transform playerTransform)
    {
        this.existsPreviousHands = new bool[2];
        this.existsPreviousHands.Fill(false);
        this.isOpenedPreviousHands = new bool[2];
        this.isOpenedPreviousHands.Fill(false);
        this.isOpenedPreviousFingers = new bool[2, 6]; //thumb, index, middle, ring, pinky, unknown
        this.isOpenedPreviousFingers.Fill(false);
        this.handIds = new int[2] { HandUtil.LEFT, HandUtil.RIGHT };
        this.playerTransform = playerTransform;
    }

    /*
     * 
     * Leap Motionのframe.Handsのindexはleft, rightに対応していないため
     *
     * @param
     *     frame: One frame
     * @return
     *     hand[0]: Left hand
     *     hand[1]: Right hand
     */
    public static Hand[] GetCorrectHands(Frame frame)
    {
        Hand[] hands = new Hand[2];
        if (frame.Hands.Count >= 2) {
            hands[LEFT] = frame.Hands[0].IsLeft ? frame.Hands[0] : frame.Hands[1];
            hands[RIGHT] = frame.Hands[1].IsRight ? frame.Hands[1] : frame.Hands[0];
        }
        else if (frame.Hands.Count == 1) {
            hands[LEFT] = frame.Hands[0].IsLeft ? frame.Hands[0] : null;
            hands[RIGHT] = frame.Hands[0].IsRight ? frame.Hands[0] : null; }
        else { hands[LEFT] = hands[RIGHT] = null; }
        return hands;
    }

    /*
     * Returns whether the hand is grabbed (four fingers are closed).
     *
     * Check out GrabStreangth variable of Hand class
     * https://leapmotion.github.io/UnityModules/class_leap_1_1_hand.html#ae3b86d4d13139d772be092b6838ee9b5
     */
    public static HandStatus GetHandStatus(Hand hand)
    {
        //The strength is zero for an open hand
        if (hand.GrabStrength == 0.0f) { return HandStatus.OPEN; }
        else if (hand.GrabStrength == 1.0f) { return HandStatus.CLOSE; }
        else { return HandStatus.UNKNOWN; }
    }

    /*
     * Save whether your previous hands are opened
     * THIS IS USED FOR JustOpenedHandOn() & JustClosedHandOn()
     * @param hands Leap.Hand[] both hands
     */
    public void SavePreviousHands(Hand[] hands)
    {
        foreach (int handId in this.handIds) {
            Hand hand = hands[handId];
            if (hand == null) {
                this.existsPreviousHands[handId] = false;
            }
            else {
                this.existsPreviousHands[handId] = true;
                if (HandUtil.GetHandStatus(hand) == HandStatus.OPEN) {
                    this.isOpenedPreviousHands[handId] = true;
                }
                else if (HandUtil.GetHandStatus(hand) == HandStatus.CLOSE) {
                    this.isOpenedPreviousHands[handId] = false;
                }
                else { } //何も保存しない
            }
        }
        //Debug.Log("Left: " + this.isOpenedPreviousHands[HandUtil.LEFT]);
        //Debug.Log("Right: " + this.isOpenedPreviousHands[HandUtil.RIGHT]);
    }

    /*
     * Save whether your previous fingers are opened
     * THIS IS USED FOR JustOpenedFingerOn(), JustClosedFingerOn().
     * @param hands Leap.Hand[] both hands
     */
    public void SavePreviousFingers(Hand[] hands)
    {
        foreach (int handId in this.handIds) {
            Hand hand = hands[handId];
            if (hand != null) {
                foreach (Finger f in hand.Fingers) {
                    this.isOpenedPreviousFingers[handId, (int)f.Type] = f.IsExtended;
                }
            }
        }
    }

    /*
    public bool JustAppearedHand(Hand[] hands, int handId) {
        return !this.existsPreviousHands[handId] && hands[handId] != null;
    }
    public bool JustDisappearedHand(Hand[] hands, int handId) {
        return this.existsPreviousHands[handId] && hands[handId] == null;
    }
    */

    /*
     * Returns whether a hand is just opened from a state of closed hand
     * @param hand: LeapMotion Hand Model
     * @param handId: HandUtil.LEFT(=0) or HandUtil.RIGHT(=1)
     * @return whether it's opened (true or false)
     */
    public bool JustOpenedHandOn(Hand[] hands, int handId) {
        return this.JustActionedHandOn(hands, handId, HandActionType.JUST_OPENED);
    }

    /*
     * Returns whether a hand is just closed from a state of opened hand
     * @param hand: LeapMotion Hand Model
     * @param handId: HandUtil.LEFT(=0) or HandUtil.RIGHT(=1)
     * @return whether it's opened (true or false)
     */
    public bool JustClosedHandOn(Hand[] hands, int handId) {
        return this.JustActionedHandOn(hands, handId, HandActionType.JUST_CLOSED);
    }

    /*
     * Returns whether a hand is "just opened" or "just closed" from a state of closed or opened hand
     *
     * Called by JustOpenedHandOn  or JustClosedHandOn 
     * @param hand: LeapMotion Hand Model
     * @param handId: HandUtil.LEFT(=0) or HandUtil.RIGHT(=1)
     * @param actionType: Hand just opened action or Hand just closed action
     * @return whether the action is started (true or false)
     */
    private bool JustActionedHandOn(Hand[] hands, int handId, HandActionType actionType) {
        Hand hand = hands[handId];
        //過去手を開いていて，現在の手が存在し，その手の指が全部閉じるとき
        if (hand != null) {
            switch (actionType) {
                case HandActionType.JUST_OPENED:
                    //Whether the hand is just opened
                    if ( ! this.isOpenedPreviousHands[handId] && HandUtil.GetHandStatus(hand) == HandStatus.OPEN) {
                        this.isOpenedPreviousHands[handId] = true; //hand status
                        return true; //Just opened
                    }
                    break;
                case HandActionType.JUST_CLOSED:
                    //Whether the hand is just closed
                    if (this.isOpenedPreviousHands[handId] && HandUtil.GetHandStatus(hand) == HandStatus.CLOSE) {
                        this.isOpenedPreviousHands[handId] = false; //hand status
                        return true; //Just closed
                    }
                    break;
            }
        }
        return false; //Not Actioned
    }

    /*
     * Returns whether a finger is "just opened" from a state of closed finger
     * @param hand: LeapMotion Hand Model
     * @param handId: HandUtil.LEFT(=0) or HandUtil.RIGHT(=1)
     * @param fingerId: Leap.Finger.FingerType 0 ~ 4 (thumb finger to pinky finger)
     * @return whether it's just opened (true or false)
     */
    public bool JustOpenedFingerOn(Hand[] hands, int handId, int fingerId) {
        return this.JustActionedFingerOn(hands, handId, fingerId, FingerActionType.JUST_OPENED);
    }

    /*
     * Returns whether a finger is "just closed" from a state of opened finger
     * @param hand: LeapMotion Hand Model
     * @param handId: HandUtil.LEFT(=0) or HandUtil.RIGHT(=1)
     * @param fingerId: Leap.Finger.FingerType 0 ~ 4 (thumb finger to pinky finger)
     * @return whether it's just closed (true or false)
     */
    public bool JustClosedFingerOn(Hand[] hands, int handId, int fingerId) {
        return this.JustActionedFingerOn(hands, handId, fingerId, FingerActionType.JUST_CLOSED);
    }

    /*
     * Returns whether a finger is "just opened" or "just closed" from a state of closed or opened finger
     *
     * Called by JustOpenedFingerOn  or JustClosedFingerOn 
     * @param hand: LeapMotion Hand Model
     * @param handId: HandUtil.LEFT(=0) or HandUtil.RIGHT(=1)
     * @param fingerId: Leap.Finger.FingerType 0 ~ 4 (thumb finger to pinky finger)
     * @param actionType: Hand just opened action or Hand just closed action
     * @return whether the action is started (true or false)
     */
    private bool JustActionedFingerOn(Hand[] hands, int handId, int fingerId, FingerActionType actionType) {
        Hand hand = hands[handId];
        if (hand != null) {
            Finger finger = hands[handId].Fingers[fingerId];
            switch(actionType) {
                case FingerActionType.JUST_OPENED:
                     //Whether the finger is just opened
                    if  ( ! this.isOpenedPreviousFingers[handId, fingerId] && finger.IsExtended) {
                        this.isOpenedPreviousFingers[handId, fingerId] = true; //opened status
                        return true; //just opened?
                    }
                    break;
                case FingerActionType.JUST_CLOSED:
                     //Whether the finger is just closed
                    if (this.isOpenedPreviousFingers[handId, fingerId] && ! finger.IsExtended) {
                        this.isOpenedPreviousFingers[handId, fingerId] = false; //closed status
                        return true; //just closed?
                    }
                    break;
            }
        }
        return false; //No actioned
    }

    private Vector3 GetHandVelocity(Hand hand) {
        return this.playerTransform.InverseTransformDirection(HandUtil.ToVector3(hand.PalmVelocity));
    }

    public bool IsMoveLeft(Hand hand) {
        return ! this.IsHandStayed(hand) && GetHandVelocity(hand).x < -this.deltaVelocity;
    }

    public bool IsMoveRight(Hand hand) {
        return ! this.IsHandStayed(hand) && GetHandVelocity(hand).x > this.deltaVelocity;
    }

    public bool IsMoveUp(Hand hand) {
        return ! this.IsHandStayed(hand) && hand.PalmVelocity.y > this.deltaVelocity;
    }

    public bool IsMoveDown(Hand hand) {
        return ! this.IsHandStayed(hand) && hand.PalmVelocity.y < -this.deltaVelocity;
    }

    private bool IsHandStayed(Hand hand) {
        return hand.PalmVelocity.Magnitude < this.smallestVelocity;
    }

    private bool IsAlmostOppositeDirection(Vector a, Vector b) {
        return this.IsAlmostOppositeDirection(HandUtil.ToVector3(a), HandUtil.ToVector3(b));
    }
    private bool IsAlmostOppositeDirection(Vector3 a, Vector3 b) {
        return Vector3.Angle(a, b) > (180.0f - handForwardDegree);
    }

    private bool IsAlmostSameDirection(Vector a, Vector b) {
        return this.IsAlmostSameDirection(HandUtil.ToVector3(a), HandUtil.ToVector3(b));
    }
    private bool IsAlmostSameDirection(Vector3 a, Vector3 b) {
        return Vector3.Angle(a, b) < handForwardDegree;
    }

    public bool IsHandConfidence(Hand hand) {
        return hand.Confidence > 0.5f;
    }
    public bool IsGrabHand(Hand hand) {
        return hand.GrabStrength > 0.8f;
    }
    public bool IsOpenHand(Hand hand) {
        return hand.GrabStrength == 0;
    }

    private bool IsPalmNormalSameDirectionWith(Hand hand, Vector3 dir) {
        return this.IsAlmostSameDirection(HandUtil.ToVector3(hand.PalmNormal), dir);
    }

    private bool IsHandMoveForward(Hand hand) {
        return this.IsAlmostSameDirection(hand.PalmNormal, hand.PalmVelocity) && ! IsHandStayed(hand);
    }


    //手をたたいているか判定
    public bool IsCrapGesture(Hand[] hands)
    {
        Hand leftHand = hands[HandUtil.LEFT];
        Hand rightHand = hands[HandUtil.RIGHT];
        if (leftHand != null & rightHand != null) {
            if (IsOpenHand(leftHand) && IsOpenHand(rightHand)
                && IsAlmostOppositeDirection(leftHand.PalmNormal, rightHand.PalmNormal)
                && IsAlmostOppositeDirection(leftHand.PalmVelocity, rightHand.PalmVelocity)
                && IsHandMoveForward(leftHand) && IsHandMoveForward(rightHand)) {
                return true;
            }
        }
        return false;
    }

    //投げる動作か判定
    public bool IsThrownGesture(Hand hand)
    {
        if (hand != null) {
            if (this.IsPalmNormalSameDirectionWith (hand, HandUtil.ToVector3 (hand.PalmVelocity))
               && !this.IsHandStayed(hand)) {
                return true;
            }
        }
        return false;
    }

    //手のひらが上を向いているか判定
    public bool IsFaceUpGesture(Hand hand)
    {
        if (hand != null) {
            if (this.IsHandStayed(hand) && this.IsPalmNormalSameDirectionWith(hand, Vector3.up)) return true;
        }
        return false;
    }

    //手のひらが下を向いているか判定
    public bool IsFaceDownGesture(Hand hand)
    {
        if (hand != null) {
            if (this.IsHandStayed(hand) && this.IsPalmNormalSameDirectionWith(hand, Vector3.down)) return true;
        }
        return false;
    }

    //Leap Vector to Unity Vector3
    public static Vector3 ToVector3(Vector v) {
        return new Vector3(v.x, v.y, v.z);
    }

    //Leap Quaternion to Unity Quaternion
    public static Quaternion ToQuaternion(LeapQuaternion q) {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }
}
