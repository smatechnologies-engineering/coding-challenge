require(`${process.cwd()}/report/report`);

const maintainability = __report.summary.total.maintainability;

if(maintainability < 70) {
  console.log(`Plato reported total maintainability as ${maintainability}`);
}

process.exitCode = maintainability >= 70 ? 0 : 1;
