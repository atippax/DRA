function sendPost(controller, body) {
  return fetch(`http://localhost:5170/${controller}`, {
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
  return fetch(`http://localhost:5170/${controller}`, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then((x) => x.json())
    .then((s) => s);
}
async function quizSCenario() {
  await sendDelete("areas");
  await sendDelete("trucks");
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
quizSCenario();
