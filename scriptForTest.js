const baseUrl = "https://dra-service-hzfjhyhqhthqhpd5.eastasia-01.azurewebsites.net";
function sendPost(controller, body) {
  return fetch(`${baseUrl}/api/${controller}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: body != undefined ? JSON.stringify(body) : null,
  })
    .then((x) => x.json())
    .then((s) => s);
}
function sendDelete(controller) {
  return fetch(`${baseUrl}/api/${controller}`, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
    },
  }).then((x) => x);
}
async function quizScenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
  await sendDelete("assignments/reset");
  const areaOne = await sendPost("areas", {
    requiredResource: {
      food: 200,
      water: 300,
    },
    urgencyLevel: 5,
    timeConstraint: 6,
  });
  const areaTwo = await sendPost("areas", {
    requiredResource: {
      medicine: 50,
    },
    urgencyLevel: 4,
    timeConstraint: 4,
  });
  const truckOne = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 5,
      [`A${areaTwo.id}`]: 3,
    },
    resources: {
      food: 250,
      water: 300,
    },
  });
  const truckTwo = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 4,
      [`A${areaTwo.id}`]: 2,
    },
    resources: {
      medicine: 60,
    },
  });
  const assignment = await sendPost("assignments");
  console.log(assignment);
}
async function notEnoughtAnyResourcesScenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
  await sendDelete("assignments/reset");
  const areaOne = await sendPost("areas", {
    requiredResource: {
      food: 200,
      water: 3000,
    },
    urgencyLevel: 5,
    timeConstraint: 6,
  });
  const areaTwo = await sendPost("areas", {
    requiredResource: {
      medicine: 500,
    },
    urgencyLevel: 4,
    timeConstraint: 4,
  });
  const truckOne = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 5,
      [`A${areaTwo.id}`]: 3,
    },
    resources: {
      food: 250,
      water: 300,
    },
  });
  const truckTwo = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 4,
      [`A${areaTwo.id}`]: 2,
    },
    resources: {
      medicine: 60,
    },
  });
  const assignment = await sendPost("assignments");
  console.log(assignment);
}
async function notEnoughtAnyTimeScenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
  await sendDelete("assignments/reset");
  const areaOne = await sendPost("areas", {
    requiredResource: {
      food: 200,
      water: 300,
    },
    urgencyLevel: 5,
    timeConstraint: 6,
  });
  const areaTwo = await sendPost("areas", {
    requiredResource: {
      medicine: 50,
    },
    urgencyLevel: 4,
    timeConstraint: 4,
  });
  const truckOne = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 10,
      [`A${areaTwo.id}`]: 10,
    },
    resources: {
      food: 250,
      water: 300,
    },
  });
  const truckTwo = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 10,
      [`A${areaTwo.id}`]: 10,
    },
    resources: {
      medicine: 60,
    },
  });
  const assignment = await sendPost("assignments");
  console.log(assignment);
}
async function mixScenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
  await sendDelete("assignments/reset");
  const [areaOne, areaTwo, areaThree] = await Promise.all([
    await sendPost("areas", {
      requiredResource: {
        food: 2000,
        water: 3000,
      },
      urgencyLevel: 5,
      timeConstraint: 6,
    }),
    await sendPost("areas", {
      requiredResource: {
        medicine: 50,
      },
      urgencyLevel: 4,
      timeConstraint: 1,
    }),
    await sendPost("areas", {
      requiredResource: {
        medicine: 50,
      },
      urgencyLevel: 4,
      timeConstraint: 5,
    }),
    await sendPost("areas", {
      requiredResource: {
        medicine: 500,
      },
      urgencyLevel: 4,
      timeConstraint: 1,
    }),
  ]);
  Promise.all([
    await sendPost("trucks", {
      timeToTravel: {
        [`A${areaOne.id}`]: 4,
        [`A${areaTwo.id}`]: 2,
        [`A${areaThree.id}`]: 2,
      },
      resources: {
        food: 250,
        water: 300,
      },
    }),
    await sendPost("trucks", {
      timeToTravel: {
        [`A${areaOne.id}`]: 4,
        [`A${areaTwo.id}`]: 2,
        [`A${areaThree.id}`]: 4,
      },
      resources: {
        medicine: 60,
      },
    }),
    await sendPost("trucks", {
      timeToTravel: {
        [`A${areaOne.id}`]: 10,
        [`A${areaTwo.id}`]: 10,
      },
      resources: {
        medicine: 60,
      },
    }),
  ]);

  const assignment = await sendPost("assignments");
  console.log(assignment);
}
async function noAreaScenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
  await sendDelete("assignments/reset");
  const areaOne = await sendPost("areas", {
    requiredResource: {
      food: 200,
      water: 300,
    },
    urgencyLevel: 5,
    timeConstraint: 6,
  });
  const areaTwo = await sendPost("areas", {
    requiredResource: {
      medicine: 50,
    },
    urgencyLevel: 4,
    timeConstraint: 4,
  });
  await sendDelete("areas");
  const truckOne = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 5,
      [`A${areaTwo.id}`]: 3,
    },
    resources: {
      food: 250,
      water: 300,
    },
  });
  const truckTwo = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 4,
      [`A${areaTwo.id}`]: 2,
    },
    resources: {
      medicine: 60,
    },
  });
  const assignment = await sendPost("assignments");
  console.log(assignment);
}
async function noTruckScenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
  await sendDelete("assignments/reset");
  const areaOne = await sendPost("areas", {
    requiredResource: {
      food: 200,
      water: 300,
    },
    urgencyLevel: 5,
    timeConstraint: 6,
  });
  const areaTwo = await sendPost("areas", {
    requiredResource: {
      medicine: 50,
    },
    urgencyLevel: 4,
    timeConstraint: 4,
  });
  const truckOne = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 5,
      [`A${areaTwo.id}`]: 3,
    },
    resources: {
      food: 250,
      water: 300,
    },
  });
  const truckTwo = await sendPost("trucks", {
    timeToTravel: {
      [`A${areaOne.id}`]: 4,
      [`A${areaTwo.id}`]: 2,
    },
    resources: {
      medicine: 60,
    },
  });
  await sendDelete("trucks");
  const assignment = await sendPost("assignments");
  console.log(assignment);
}
// quizScenario();
// notEnoughtAnyResourcesScnario();
// notEnoughtAnyTimeScenario();
// noAreaScenario();
// noTruckScenario();
