const a = fetch("http://localhost:5170/assignments", {
  method: "POST",
  headers: {
    "Content-Type": "application/json",
  },
  body: JSON.stringify({
    requiredResource: {
      water: 2,
    },
    urgencyLevel: 5,
    timeConstraint: 2,
  }),
})
  .then((x) => x.json())
  .then((s) => console.log(s));
