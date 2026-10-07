# Product and Operator Validation

The product is validated against real operator outcomes, not only implementation correctness.

## Before coding a business capability

Define:
- primary user/actor;
- real-world job to be completed;
- current pain/problem;
- desired outcome;
- acceptable completion time;
- acceptable error/recovery path;
- permissions and accountability;
- evidence that the workflow is useful.

## Validation method

For every major capability:
1. write the operator workflow;
2. identify the smallest useful outcome;
3. review the workflow before UI implementation;
4. test it with realistic shop scenarios;
5. record rejected/changed assumptions;
6. update the requirement and traceability row.

## Operator-success metrics

Examples:
- time to start a session;
- time to settle a session;
- number of steps for common operations;
- error/retry rate;
- frequency of manual corrections;
- time to diagnose a disconnected Agent;
- time to recover from a failed payment/update;
- number of permission-related interruptions.

The exact targets are approved per release rather than invented by the UI implementation.

## Feedback rule

Product feedback that changes scope or architecture goes through requirements traceability and change control. It does not get inserted directly into a screen because it is easy.

## Release acceptance

A feature can be technically complete but still fail operator acceptance. Both are required.
