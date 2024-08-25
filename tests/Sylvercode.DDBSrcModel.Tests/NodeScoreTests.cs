using Sylvercode.DDBSrcModel.StructDocStack.Score;
using Xunit.Sdk;

namespace Sylvercode.DDBSrcModel.Tests;

public class NodeScoreTests_SubScore_CompareTo
{
  [Theory]
  [InlineData(1, 1, 2, 2)]
  [InlineData(1, 1, 1, 2)]
  public void LeftSmaller_ReturnNegative(int priority1, int value1, int priority2, int value2)
  {
    // Given
    NodeScore.SubScore leftSubScore = new(priority1, value1);
    NodeScore.SubScore rightSubScore = new(priority2, value2);

    // When
    int result = leftSubScore.CompareTo(rightSubScore);

    // Then
    Assert.True(int.IsNegative(result));
  }

  [Theory]
  [InlineData(2, 2, 1, 1)]
  [InlineData(2, 2, 2, 1)]
  public void RightSmaller_ReturnPositive(int priority1, int value1, int priority2, int value2)
  {
    // Given
    NodeScore.SubScore leftSubScore = new(priority1, value1);
    NodeScore.SubScore rightSubScore = new(priority2, value2);

    // When
    int result = leftSubScore.CompareTo(rightSubScore);

    // Then
    Assert.True(int.IsPositive(result));
  }

  [Theory]
  [InlineData(1, 1, 1, 1)]
  public void BothEquals_ReturnZero(int priority1, int value1, int priority2, int value2)
  {
    // Given
    NodeScore.SubScore leftSubScore = new(priority1, value1);
    NodeScore.SubScore rightSubScore = new(priority2, value2);

    // When
    int result = leftSubScore.CompareTo(rightSubScore);

    // Then
    Assert.Equal(0, result);
  }
}

public class NodeScoreTests_CompareTo
{
  static NodeScore.SubScore SubScore_1_1 = new(1, 1);
  static NodeScore.SubScore SubScore_1_2 = new(1, 2);
  static NodeScore.SubScore SubScore_2_1 = new(2, 1);
  static NodeScore.SubScore SubScore_2_2 = new(2, 2);

  public static IEnumerable<object[]> LeftSubScoresSmaller = [[ new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_1 },   // First Smaller
                                                                new NodeScore.SubScore[] { SubScore_1_2, SubScore_2_2 } ],
                                                              [ new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_1 }, // Second Smaller
                                                                new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_2 } ],
                                                              [ new NodeScore.SubScore[] { SubScore_1_1 }, // Priority smaller
                                                                new NodeScore.SubScore[] { SubScore_2_1 } ],
                                                              [ new NodeScore.SubScore[] { SubScore_1_1 }, // Less SubScore
                                                                new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_2 } ], // Second Smaller
                                                              [ new NodeScore.SubScore[] { }, // No SubScore
                                                                new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_2 } ]
                                                             ];

  [Theory]
  [MemberData(nameof(LeftSubScoresSmaller))]
  public void LeftSmaller_ReturnNegative(NodeScore.SubScore[] SubScoresLeft, NodeScore.SubScore[] SubScoresRight)
  {
    // Given
    NodeScore left = new(SubScoresLeft);
    NodeScore right = new(SubScoresRight);

    // When 
    int result = left.CompareTo(right);

    // Then
    Assert.True(int.IsNegative(result));
  }

  public static IEnumerable<object[]> RightSubScoresSmaller = [[ new NodeScore.SubScore[] { SubScore_1_2, SubScore_2_2 },
                                                                 new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_1 } ], // First Smaller
                                                               [ new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_2 },
                                                                 new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_1 } ], // Second Smaller
                                                               [ new NodeScore.SubScore[] { SubScore_2_1 },
                                                                 new NodeScore.SubScore[] { SubScore_1_1 } ], // Priority smaller
                                                               [ new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_2 },
                                                                 new NodeScore.SubScore[] { SubScore_1_1 } ], // Less SubScore
                                                               [ new NodeScore.SubScore[] { SubScore_1_1, SubScore_2_2 },
                                                                 new NodeScore.SubScore[] { } ] // No SubScore
                                                              ];

  [Theory]
  [MemberData(nameof(RightSubScoresSmaller))]
  public void RightSmaller_ReturnPositive(NodeScore.SubScore[] SubScoresLeft, NodeScore.SubScore[] SubScoresRight)
  {
    // Given
    NodeScore left = new(SubScoresLeft);
    NodeScore right = new(SubScoresRight);

    // When 
    int result = left.CompareTo(right);

    // Then
    Assert.True(int.IsPositive(result));
  }

  public static IEnumerable<object[]> BothSubScoresEquals = [[ new NodeScore.SubScore[] { SubScore_1_2, SubScore_2_2 },
                                                               new NodeScore.SubScore[] { SubScore_1_2, SubScore_2_2 } ],
                                                             [ new NodeScore.SubScore[] { SubScore_1_1 },
                                                               new NodeScore.SubScore[] { SubScore_1_1 } ],
                                                             [ new NodeScore.SubScore[] { },
                                                               new NodeScore.SubScore[] { } ]
                                                            ];

  [Theory]
  [MemberData(nameof(BothSubScoresEquals))]
  public void BothEquals_ReturnZero(NodeScore.SubScore[] SubScoresLeft, NodeScore.SubScore[] SubScoresRight)
  {
    // Given
    NodeScore left = new(SubScoresLeft);
    NodeScore right = new(SubScoresRight);

    // When 
    int result = left.CompareTo(right);

    // Then
    Assert.Equal(0, result);
  }
}
